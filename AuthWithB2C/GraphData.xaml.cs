using System;
using Microsoft.Identity.Client;
using Xamarin.Forms;

namespace AuthWithB2C
{
    public partial class GraphData : ContentPage
    {
        private readonly AuthenticationResult authenticationResult;

        public GraphData(AuthenticationResult result)
        {
            InitializeComponent();
            authenticationResult = result;
        }

        protected override void OnAppearing()
        {
            if (authenticationResult != null)
            {
                if (authenticationResult.Account.Username != "unknown")
                {
                    messageLabel.Text = string.Format("Welcome {0}", authenticationResult.Account.Username);
                }
                else
                {
                    messageLabel.Text = string.Format("UserId: {0}", authenticationResult.Account.Username);
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
