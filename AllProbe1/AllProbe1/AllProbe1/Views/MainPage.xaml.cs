using AllProbe1.Droid;
using AllProbe1.Models;
using AllProbe1.Services;
using AllProbe1.ViewModels;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
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

                ///The landing tab is decided here
                ///according to "orientation" field that returned from sever.
                ///See Services.cs->ParseLoginJSON func.
                switch (GlobalServices.GetOrientation())
                {
                    case 0: //orientation=none
                    case 1: //orientation=events
                        CurrentPage = this.Children[0];
                        break;
                    case 2: //orientation=websites
                    case 3: //orientation=website
                        CurrentPage = this.Children[1];
                        break;
                    default:
                        CurrentPage = this.Children[0];
                        break;
                }

                am = (Android.App.AlarmManager)Android.App.Application.Context.GetSystemService(Context.AlarmService);
                Intent intent = new Intent(Android.App.Application.Context, typeof(AlarmReceiver));
                pi = Android.App.PendingIntent.GetBroadcast(Android.App.Application.Context, 0, intent, 0);
                am.SetExact(Android.App.AlarmType.Rtc, 0, pi);

                ///Max number of tabs = 3, unless CurrentPage is the 4th tab.
                ///When CurrentPage is no longer the 4th tab, it will be removed.
                this.CurrentPageChanged += (object sender, EventArgs e) =>
                 {
                     if (this.Children.Count > 3 && (CurrentPage != this.Children[3]))
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
