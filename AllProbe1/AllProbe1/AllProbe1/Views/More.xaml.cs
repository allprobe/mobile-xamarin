using AllProbe1.Droid;
using AllProbe1.Services;
using AllProbe1.Models;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

                SessionState sessionState = SessionState.GetInstance();
                sessionState.SessionId = null;
                ICacheService cacheService = new CacheService();
                cacheService.SetCache(Android.App.Application.Context.Resources.GetString(Resource.String.sessionId), null);
                cacheService.SetCache(Android.App.Application.Context.Resources.GetString(Resource.String.isAlarmManagerRunnning), null);
                NavigationPage.SetHasNavigationBar(this, false);

                Application.Current.MainPage = new LoginPage(null);
            }            
        }
    }
}
