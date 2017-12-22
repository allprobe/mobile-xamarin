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

                am = (Android.App.AlarmManager)Android.App.Application.Context.GetSystemService(Context.AlarmService);
                Intent intent = new Intent(Android.App.Application.Context, typeof(AlarmReceiver));
                pi = Android.App.PendingIntent.GetBroadcast(Android.App.Application.Context, 0, intent, 0);
                am.SetExact(Android.App.AlarmType.Rtc, 0, pi);

                ///Max number of tabs = 3, unless CurrentPage is the 4th tab.
                ///When CurrentPage is no longer the 4th tab, it will be removed.
                this.CurrentPageChanged += (object sender, EventArgs e) =>
                 {
                     if ((CurrentPage == this.Children[0] || CurrentPage == this.Children[1] || CurrentPage == this.Children[2])
                        && this.Children.Count > 3)
                         this.Children.RemoveAt(3);
                 };
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
