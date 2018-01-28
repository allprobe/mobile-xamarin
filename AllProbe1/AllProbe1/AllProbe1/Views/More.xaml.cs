using AllProbe1.Droid;
using AllProbe1.Services;
using AllProbe1.Models;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json.Linq;

namespace AllProbe1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class More : ContentPage
    {
        private List<string> severityList = null;

        public More()
        {
            InitializeComponent();
            Initialize();
            this.lblLogout.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => Logout()), });
            this.imgLogout.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => Logout()), });
            this.lblFeedback.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => FeedbackFrame.IsVisible = true) });
            this.imgFeedback.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => FeedbackFrame.IsVisible = true) });
            this.CloseFeedback.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => ClickCloseFeedback())});

        }

        private void Initialize()
        {
            severityList = GlobalServices.SeverityList;
            foreach (string item in severityList)
                this.pkSeverity.Items.Add(item);

            object selectedSeverity = null;
            if (Application.Current.Properties.ContainsKey(Android.App.Application.Context.Resources.GetString(Resource.String.selectedSeverity)))
            {
                selectedSeverity = Application.Current.Properties[Android.App.Application.Context.Resources.GetString(Resource.String.selectedSeverity)];

                if (selectedSeverity != null)
                    this.pkSeverity.SelectedIndex = Convert.ToInt32(selectedSeverity);
            }
            else
            {
                Application.Current.Properties[Android.App.Application.Context.Resources.GetString(Resource.String.selectedSeverity)] = 0;
            }

        }

        private void SelectedItem_handler()
        {
            Application.Current.Properties[Android.App.Application.Context.Resources.GetString(Resource.String.selectedSeverity)] = this.pkSeverity.SelectedIndex;
        }

        async private void Logout()
        {
            if (await DisplayAlert("Logout", "Are you sure?", "Logout", "Cancel"))
            {
                MainPage.pi.Cancel();
                MainPage.am.Cancel(MainPage.pi);
                GlobalServices.SetUserEmailAddress(null);

                SessionState sessionState = SessionState.GetInstance();
                sessionState.SessionId = null;
                ICacheService cacheService = new CacheService();
                cacheService.SetCache(Android.App.Application.Context.Resources.GetString(Resource.String.sessionId), null);
                cacheService.SetCache(Android.App.Application.Context.Resources.GetString(Resource.String.isAlarmManagerRunnning), null);
                NavigationPage.SetHasNavigationBar(this, false);

                Application.Current.MainPage = new LoginPage(null);
            }
        }

        private async void ClickSendFeedback(object sender, EventArgs e)
        {
            try
            {
                if (FeedbackEditor.Text==null || FeedbackEditor.Text.Length < 3)
                    throw new ArgumentException("Please elaborate. Feedback hasn't been sent.");
                IServices services = new Services.Services();
                JObject FeedbackAnswer = await services.PostFeedback(GlobalServices.GetUserEmailAddress(), FeedbackEditor.Text as string);
                if (Convert.ToString(FeedbackAnswer["msg"]) == "success") {
                    Android.Widget.Toast.MakeText(Android.App.Application.Context, Convert.ToString(FeedbackAnswer["data"]["msg"]), Android.Widget.ToastLength.Long).Show();
                    FeedbackEditor.Text = null;
                    FeedbackFrame.IsVisible = false;
                }
                else
                    throw new ApplicationException("There was a problem. Feedback hasn't been sent.");
            }
            catch (Exception ex)
            {
                FeedbackError.Text = ex.Message;
                FeedbackError.IsVisible = true;
            }
        }

        private void ClickCloseFeedback()
        {
            FeedbackEditor.Text = null;
            FeedbackError.Text = null;
            FeedbackError.IsVisible = false;
            FeedbackFrame.IsVisible = false;
        }
    }
}
