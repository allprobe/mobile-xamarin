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
    public partial class MainPage : TabbedPage
    {
        public static Android.App.AlarmManager am = null;
        public static Android.App.PendingIntent pi = null;

        public MainPage()
        {
            try
            {
                InitializeComponent();
                
                //Title = "AllProbe\t" + DateTime.Now.ToShortTimeString();

                am = (Android.App.AlarmManager)Forms.Context.GetSystemService(Context.AlarmService);
                Intent intent = new Intent(Forms.Context, typeof(AlarmReceiver));
                pi = Android.App.PendingIntent.GetBroadcast(Forms.Context, 0, intent, 0);
                am.SetExact(Android.App.AlarmType.Rtc, 0, pi);
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
