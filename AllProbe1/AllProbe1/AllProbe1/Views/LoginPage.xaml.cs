using AllProbe1.Droid;
using AllProbe1.Models;
using AllProbe1.Services;
using Android.Content;
using Newtonsoft.Json.Linq;
using System;
using System.Json;
using System.Net.Mail;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllProbe1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private string sessionId = null;
        private IServices services = new Services.Services();
        private ICacheService cacheService = new CacheService();

        public LoginPage(string error)
        {
            InitializeComponent();
            if (error != null)
                this.messageLabel.Text = error;
        }

        private void ClickLoginWithFacebook(object sender, EventArgs e)
        {
            OAuth2Authenticator auth = new OAuth2Authenticator(
                clientId: "475278706153922",  // it is valid! I checked
                clientSecret: "97d1f57c222d0f6349300b35dd2ff9ca",
                accessTokenUrl: new Uri("https://graph.facebook.com/oauth/access_token"),
                scope: "",  // the scopes for the particular API you're accessing, delimited by "+" symbols
                authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),  // the auth URL for the service
                redirectUrl: new Uri("https://www.facebook.com/connect/login_success.html"));  // the redirect URL for the service

            //OAuth2Authenticator auth = new OAuth2Authenticator
            //        (
            //            clientId: "475278706153922",
            //            scope: "",
            //            authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
            //            redirectUrl: new Uri("https://www.facebook.com/connect/login_success.html"),

            //            // switch for new Native UI API
            //            //      true = Android Custom Tabs and/or iOS Safari View Controller
            //            //      false = Embedded Browsers used (Android WebView, iOS UIWebView)
            //            //  default = false  (not using NEW native UI)
            //            isUsingNativeUI: false
            //        );

            auth.Completed += Auth_Completed;
            global::Android.Content.Intent ui_object = auth.GetUI(Android.App.Application.Context);
            Android.App.Application.Context.StartActivity(ui_object);
        }

        private async void Auth_Completed(object sender, AuthenticatorCompletedEventArgs e)
        {
            // UI presented, so it's up to us to dimiss it on Android
            // dismiss Activity with WebView or CustomTabs
            //Navigation.RemovePage(this);

            if (e.IsAuthenticated)
            {
                var request = new OAuth2Request(
                    "GET",
                    new Uri("https://graph.facebook.com/v2.11/me?fields=name,email,picture,id,outbox"),
                    null,
                    e.Account
                    );

                var fbResponse = await request.GetResponseAsync();
                var fbUser = JObject.Parse(fbResponse.GetResponseText());

                StartMenu(fbUser);
            }
            else
            {
                // The user cancelled
            }



            // For IOS
            //auth.Completed += (sender, eventArgs) =>
            //{
            //    // UI presented, so it's up to us to dimiss it on iOS
            //    // dismiss ViewController with UIWebView or SFSafariViewController
            //    this.DismissViewController(true, null);

            //    if (eventArgs.IsAuthenticated)
            //    {
            //        // Use eventArgs.Account to do wonderful things
            //    }
            //    else
            //    {
            //        // The user cancelled
            //    }
            //};
        }

        private void StartMenu(JObject user)
        {
            string name = null;
            string id = null;
            string picture = null;
            string email = null;
            string outbox = null;

            if (user["name"] != null)
                name = user["name"].ToString();
            if (user["id"] != null)
                id = user["id"].ToString();
            if (user["picture"] != null && user["picture"]["data"] != null && user["picture"]["data"]["url"] != null)
                picture = user["picture"]["data"]["url"].ToString();
            if (user["email"] != null)
                email = user["email"].ToString();
            if (user["outbox"] != null)
                outbox = user["outbox"].ToString();

            //PictureImage.Image;
            if (email != null)
                sessionId = services.Login(email, id);
            else
                sessionId = services.Login(outbox, id);
            AfterLogin(sessionId);
        }

        private void ClickLogin(object sender, EventArgs e)
        {
            try
            {
                ///this.emailEntry.Text = "demo@allprobe.com";
                ///this.passwordEntry.Text = "wwwwwww";

                this.messageLabel.Text = string.Empty;
                if (string.IsNullOrEmpty(this.emailEntry.Text))
                {
                    this.messageLabel.Text = Android.App.Application.Context.Resources.GetString(Droid.Resource.String.emailMandatory);
                    return;
                }
                else if (string.IsNullOrEmpty(this.passwordEntry.Text))
                {
                    this.messageLabel.Text = Android.App.Application.Context.Resources.GetString(Droid.Resource.String.passwordMandatory);
                    return;
                }

                string email = this.emailEntry.Text.Trim();
                string password = this.passwordEntry.Text;

                if (email.Equals(""))
                {
                    this.messageLabel.Text = Android.App.Application.Context.Resources.GetString(Droid.Resource.String.emailMandatory);
                    return;

                }
                else if (!GlobalServices.IsValid(email))
                {
                    this.messageLabel.Text = Android.App.Application.Context.Resources.GetString(Droid.Resource.String.invalidEmail);
                    return;
                }
                else if (string.IsNullOrEmpty(password))
                {
                    this.messageLabel.Text = Android.App.Application.Context.Resources.GetString(Droid.Resource.String.passwordMandatory);
                    return;
                }

                sessionId = services.Login(email, password);
                AfterLogin(sessionId);
            }
            catch (Exception ex)
            {
                this.messageLabel.Text = ex.Message;
            }
        }

        private void AfterLogin(string sessionId)
        {

            SessionState sessionState = SessionState.GetInstance();
            sessionState.SessionId = sessionId;
            cacheService.SetCache(Android.App.Application.Context.Resources.GetString(Droid.Resource.String.sessionId), sessionId);

            if (sessionId != null && sessionId.StartsWith("!"))
            {
                this.messageLabel.Text = sessionId;
            }
            else if (sessionId != null)
            {
                cacheService.SetCache(Android.App.Application.Context.Resources.GetString(Droid.Resource.String.isAlarmManagerRunnning), null);
                cacheService.SetCache(Android.App.Application.Context.Resources.GetString(Droid.Resource.String.sessionId), sessionId);

                Application.Current.MainPage = new NavigationPage(new MainPage());
            }
            else
            {
                this.messageLabel.Text = Android.App.Application.Context.Resources.GetString(Droid.Resource.String.incorrectEmailPasswordCombination);
            }
        }
    }
}
