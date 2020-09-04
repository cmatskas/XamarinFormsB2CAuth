using Microsoft.Identity.Client;
using Xamarin.Forms;

namespace AuthWithB2C
{
    public partial class App : Application
    {
        public static IPublicClientApplication AuthenticationClient { get; private set; }

        public static object UIParent { get; set; } = null;

        public App()
        {
            InitializeComponent();

            AuthenticationClient = PublicClientApplicationBuilder.Create(Constants.ClientId)
                                        .WithIosKeychainSecurityGroup(Constants.IosKeychainSecurityGroup)
                                        .WithB2CAuthority(Constants.AuthoritySignIn)
                                        .WithRedirectUri($"msal{Constants.ClientId}://auth")
                                        .Build();

            MainPage = new NavigationPage(new LoginPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
