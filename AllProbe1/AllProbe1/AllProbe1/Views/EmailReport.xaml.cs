using AllProbe1.Droid;
using AllProbe1.Models;
using AllProbe1.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;

namespace AllProbe1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmailReport : ContentPage
    {
        string webSite = null;
        public EmailReport(string webSite)
        {
            InitializeComponent();
            lblClose.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => lblClosePopup()), });
            this.webSite = webSite;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.Animate("FrameAppear", new Animation(v=>this.Scale=v,0,1,null,null), 400, 500, Easing.CubicIn, null, null);
            //this.Animate("scal", (s) => Layout(new Rectangle(((1 - s) * Width), Y, Width, Height)), 0, 600, Easing.BounceIn, null, null);
        }
        private void ClickSend(object sender, EventArgs e)
        {
            string email = this.emailEntry.Text;

            if (string.IsNullOrEmpty(email))
            {
                messageLabel.Text = Android.App.Application.Context.Resources.GetString(Resource.String.emailMandatory);
                return;
            }
            if (!GlobalServices.IsValid(email))
            {
                messageLabel.Text = Android.App.Application.Context.Resources.GetString(Resource.String.invalidEmail);
                return;
            }

            SessionState sessionState = SessionState.GetInstance();
            IServices services = new Services.Services();
            services.PostReport(email, webSite, sessionState.SessionId);

            ClosePopUp();
        }

        private void lblClosePopup()
        {
            ClosePopUp();
        }
        ///On this specific page, the device's BACK button (Android and WinPhone)
        ///will not exit the app. Instead it'll close the EmailReport.
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(()=> { this.Animate("FrameDisappear", new Animation(v => this.Scale = v, 1, 0, null, null), 400, 2000, Easing.BounceOut, null, null); Navigation.PopModalAsync(); });
            
            
            return true;
        }
        private void ClosePopUp()
        {
            //this.Animate("FrameDisappear", new Animation(v => this.Scale = v, 0, 1, null, null), 400, 2000, Easing.BounceOut, null, null);
            Navigation.PopModalAsync();
        }
        //protected override void OnDisappearing()
        //{
        //    base.OnDisappearing();
        //    this.Animate("FrameDisappear", new Animation(v => this.Scale = v, 1, 0, null, null), 400, 2000, Easing.BounceOut, null, null);
        //}

    }
}
