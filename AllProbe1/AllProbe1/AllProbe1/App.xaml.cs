using AllProbe1.Droid;
using AllProbe1.Models;
using AllProbe1.Services;
using AllProbe1.Views;
using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace AllProbe1
{
    public partial class App : Application
    {
        public static string AppName { get { return "AllProbe"; } }
        public string ALARM_ACTION { get; private set; }

        public App()
        {
            string error = null;
            InitializeComponent();

            SessionState sessionState = SessionState.GetInstance();
            sessionState.SessionId = null;
            ICacheService cacheService = new CacheService();
            cacheService.SetCache(Android.App.Application.Context.Resources.GetString(Resource.String.isAlarmManagerRunnning), null);

            if (!Xamarin.Forms.Application.Current.Properties.ContainsKey(Android.App.Application.Context.Resources.GetString(Resource.String.selectedSeverity)))
                Xamarin.Forms.Application.Current.Properties.Add(Android.App.Application.Context.Resources.GetString(Resource.String.selectedSeverity), 0);
            GlobalServices.setMenuSelected(0);

            if (cacheService.GetCache(Android.App.Application.Context.Resources.GetString(Resource.String.sessionId)) != null)
            {
                sessionState.SessionId = cacheService.GetCache(Android.App.Application.Context.Resources.GetString(Resource.String.sessionId)).ToString();
                MainPage = new MainPage();
            }
            else
                MainPage = new LoginPage(error);
        }
    }
}

