using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.Identity.Client;
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

        protected override void OnAppearing()
        {
            if (authenticationResult != null)
            { 
                var handler = new JwtSecurityTokenHandler();
                var data = handler.ReadJwtToken(authenticationResult.IdToken);
                var claims = data.Claims.ToList();
                if (data != null)
                {
                    welcome.Text = $"Welcome {data.Claims.FirstOrDefault(x => x.Type.Equals("name")).Value}";
                    issuer.Text = $"Issuer: { data.Claims.FirstOrDefault(x => x.Type.Equals("iss")).Value}";
                    subscription.Text = $"Subscription: {data.Claims.FirstOrDefault(x => x.Type.Equals("sub")).Value}";
                    audience.Text = $"Audience: {data.Claims.FirstOrDefault(x => x.Type.Equals("aud")).Value}";
                    email.Text = $"Email: {data.Claims.FirstOrDefault(x => x.Type.Equals("emails")).Value}";
                }   
            }

            base.OnAppearing();
        }

        async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            if (authenticationResult != null)
            {
                await App.AuthenticationClient.RemoveAsync(authenticationResult.Account);
                await Navigation.PushAsync(new LoginPage());
            }
        }
    }
}
