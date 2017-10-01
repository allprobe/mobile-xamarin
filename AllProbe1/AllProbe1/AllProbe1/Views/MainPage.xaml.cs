using AllProbe1.Droid;
using AllProbe1.Models;
using AllProbe1.Services;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace AllProbe1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        Android.App.AlarmManager am = null;
        Android.App.PendingIntent pi = null;

        public MainPage()
        {
            try
            {
                InitializeComponent();
                IsPresented = false;
                NavigationPage.SetHasNavigationBar(this, false);

                Detail = new NavigationPage(new EventsAndWebsites());
                overlay.IsVisible = true;
                overlay.IsEnabled = true;
                overlay.BackgroundColor = Color.Gray;
                //OnPropertyChanged();

                this.lblLogout.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => ClickLogout()), });
                this.lblSettings.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => ClickSettings()), });

                this.lblWebSites.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => btnWebSitesClicked()), });
                this.lblEvents.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => btnEventsClicked()), });

                //TabbedPage.CurrentPage = TabbedPage.Children[GlobalServices.getMenuSelected()];


                //    ICacheService cacheService = new CacheService();
                //    if (cacheService.GetCache(Android.App.Application.Context.Resources.GetString(Resource.String.isAlarmManagerRunnning)) == null)
                //{
                am = (Android.App.AlarmManager)Forms.Context.GetSystemService(Context.AlarmService);
                Intent intent = new Intent(Forms.Context, typeof(AlarmReceiver));
                //intent.PutExtra("ALARM_ACTION", "true");
                pi = Android.App.PendingIntent.GetBroadcast(Forms.Context, 0, intent, 0);
                am.SetExact(Android.App.AlarmType.Rtc, 0, pi);
                //am.SetRepeating(Android.App.AlarmType.Rtc, 0, 5000, pi);
                //}
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message, "OK");
                //error = ex.Message + "\n" + ex.StackTrace;
            }
        }

        //public string ALARM_ACTION { get; private set; }

        private void ClickLogout()
        {
            pi.Cancel();
            am.Cancel(pi);
           
            SessionState sessionState = SessionState.GetInstance();
            sessionState.SessionId = null;
            ICacheService cacheService = new CacheService();
            cacheService.SetCache(Android.App.Application.Context.Resources.GetString(Resource.String.sessionId), null);
            cacheService.SetCache(Android.App.Application.Context.Resources.GetString(Resource.String.isAlarmManagerRunnning), null);
            IsPresented = false;
            NavigationPage.SetHasNavigationBar(this, false);

            Application.Current.MainPage = new LoginPage(null);

        }

        private void btnWebSitesClicked()
        {
            GlobalServices.setMenuSelected(1);
            Detail = new NavigationPage(new MainPage());
            IsPresented = false;
        }

        private void btnEventsClicked()
        {
            GlobalServices.setMenuSelected(0);
            Detail = new NavigationPage(new MainPage());
            IsPresented = false;
        }

        private void ClickSettings()
        {
            Detail = new NavigationPage((new Settings()));
            IsPresented = false;
        }
    }
}
