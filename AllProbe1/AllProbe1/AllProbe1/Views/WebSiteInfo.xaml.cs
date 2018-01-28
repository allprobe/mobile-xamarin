using AllProbe1.Droid;
using AllProbe1.Models;
using AllProbe1.Services;
using AllProbe1.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllProbe1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebSiteInfo : ContentPage
    {
    private string webSite;
        private List<WebSitesResultViewModel> webSiteResult;
        public WebSiteInfo(string webSite, List<WebSitesResultViewModel> webSiteResult)
        {
            InitializeComponent();
            //Xamarin.Forms.Platform.Android.FormsAppCompatActivity.TabLayoutResource= Resource.Layout.Tabbar;
            ///Change tab's icon color according to "state_selected" status.
            //if (Convert.ToInt32(Android.OS.Build.VERSION.SdkInt) >= 23)    //Couldn't find the equivalent for android5.1 or less. Probably need to use 2 different icon files for each icon (in 2 different colors).
            //{
            //    Android.Support.V4.Graphics.Drawable.DrawableCompat.SetTintList(Android.Content.Res.Resources.System.GetDrawable(Resource.Drawable.icon_website,null), Android.Content.Res.Resources.System.GetColorStateList(Resource.Color.TabIconsColors,null));
            //}

            webSites.ItemsSource = webSiteResult;

            this.webSiteResult = webSiteResult;
            this.webSite = webSite;

            ///URL (page header) title:
            lblURL.Text = webSite;
            lblURL.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => DisplayAlert("Full URL:", webSite, "ok")) });

            ///lblStatus styling:
            lblStatus.BackgroundColor = Color.FromHex(Android.App.Application.Context.Resources.GetString(Resource.Color.allGood));
            lblStatus.Text = "ONLINE";
            int errorCount = webSiteResult.Count(w => w.Status != 3);
            if (webSiteResult.Count(w => w.Status == 3) == webSiteResult.Count)
            {
                lblStatus.BackgroundColor = Color.FromHex(Android.App.Application.Context.Resources.GetString(Resource.Color.allGood));
                lblStatus.Text = "ONLINE";
            }
            else if (errorCount < webSiteResult.Count && errorCount > 0)
            {
                lblStatus.BackgroundColor = Color.FromHex(Android.App.Application.Context.Resources.GetString(Resource.Color.partialError));
                lblStatus.Text = "PROBLEM";
            }
            else if (errorCount == webSiteResult.Count)
            {
                lblStatus.BackgroundColor = Color.FromHex(Android.App.Application.Context.Resources.GetString(Resource.Color.error));
                lblStatus.Text = "OFFLINE";
            }
            btnCode.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => Navigation.PushAsync(new CodeAnalyze(webSite))) });

            ///SLA average logic:
            List<double> AverageItems = new List<double>();
            ICacheService cacheService = new CacheService();
            string sessionId = cacheService.GetCache(Android.App.Application.Context.Resources.GetString(Resource.String.sessionId)).ToString();
            IServices services = new Services.Services();
            JObject slaJson = services.GetSlaList(sessionId, webSiteResult);
            ///Go through all data centers in "summary".
            ///For Each of them that ISN'T EMPTY - extract the "avg_sla" value from it.
            foreach (JProperty DataCenter in slaJson["summary"])
            {
                if (DataCenter.Value.ToString().IndexOf("[]") == -1)
                {
                    AverageItems.Add(DataCenter.Value["avg_sla"].ToObject<double>());
                    //Console.WriteLine("Data Center: {0}%.", DataCenter.Value["avg_sla"].ToString());
                }
            }
            if (AverageItems.Count() < 1)
            {
                throw new System.MissingFieldException("All Data centers is empty");
            }
            else
            {
                lblSLA.Text = "Daily SLA: " + AverageItems.Average() + "%";
                btnSLA.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => { try { Navigation.PushAsync(new SLA(webSite, webSiteResult)); } catch { DisplayAlert("Oops!", "We can't do that action now.\nPlease try again later.", "I will"); } }) });
            }
        }

        ///On this specific page, the device's BACK button (Android and WinPhone)
        ///will not exit the app. Instead it'll go to WebSites page.
        protected override bool OnBackButtonPressed()
        {
            (Parent as TabbedPage).CurrentPage = (Parent as TabbedPage).Children[1];
            return true;
        }
    }
}
