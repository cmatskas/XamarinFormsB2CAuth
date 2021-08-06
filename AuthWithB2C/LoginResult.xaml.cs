using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace AuthWithB2C
{
    public partial class LoginResult : ContentPage
    {
        private readonly AuthenticationResult authenticationResult;

        public LoginResult(AuthenticationResult result)
        {
            InitializeComponent();
            authenticationResult = result;
        }

        protected override async void OnAppearing()
        {
            if (authenticationResult != null)
            {
                var handler = new JwtSecurityTokenHandler();
                var data = handler.ReadJwtToken(authenticationResult.IdToken);
                
                if (data != null)
                {
                    var claims = data.Claims.ToList();
                    DisplayData displayData;
                    var idp = claims.Any(x => x.Type.Equals("idp"))
                                ? claims.First(x => x.Type.Equals("idp")).Value
                                : string.Empty;

                    var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(claims.First(x => x.Type.Equals("auth_time")).Value));
                    
                    if (idp.Contains("twitch"))
                    {
                        displayData = await GetTwitchDisplayData(claims);
                    }
                    else
                    { 
                        displayData = new DisplayData
                        {
                            Name = claims.First(x => x.Type.Equals("name")).Value,
                            Issuer = idp.Contains("twitter")? "B2C + Twitter": "B2C",
                            Subscription = claims.First(x => x.Type.Equals("sub")).Value,
                            Email = claims.First(x => x.Type.Equals("emails")).Value
                        };
                    }

                    welcome.Text = $"Welcome {displayData.Name}";
                    issuer.Text = $"Token Issuer: { displayData.Issuer}";
                    subscription.Text = $"Subscription Id: {displayData.Subscription}";
                    email.Text = $"Email: {displayData.Email}";
                    loggedinat.Text = $"User logged in at: {dateTimeOffset.ToLocalTime().ToString("g")}";
                }
            }
            base.OnAppearing();
        }

        private async Task<DisplayData> GetTwitchDisplayData(List<Claim> claims)
        {
            var email = await GetEmailFromTwitch(claims.First(x => x.Type.Equals("idp_access_token")).Value);
            return new DisplayData
            {
                Name = claims.First(x => x.Type.Equals("name")).Value,
                Issuer = "B2C + Twitch",
                Subscription = claims.First(x => x.Type.Equals("sub")).Value,
                Email = email
            };
        }

        async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            if (authenticationResult != null)
            {
                await App.AuthenticationClient.RemoveAsync(authenticationResult.Account);
                await Navigation.PushAsync(new LoginPage());
            }
        }

        async Task<string> GetEmailFromTwitch(string token)
        {
            try
            {
                var httpClient = new HttpClient();
                var message = new HttpRequestMessage(HttpMethod.Get, "https://api.twitch.tv/helix/users?login=cmatskas");
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                message.Headers.Add("Client-id", Constants.TwitchClientId);
                var response = await httpClient.SendAsync(message).ConfigureAwait(false);
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var twitchData = JObject.Parse(responseString);
                var root = twitchData["data"];
                return root.First["email"].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

    }
    public class DisplayData
    {
        public string Name { get; set; }
        public string Issuer { get; set; }
        public string Subscription { get; set; }
        public string Email { get; set; }
    };
}
