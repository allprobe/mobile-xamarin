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
        private Dictionary<string, SlaViewModel> slaSummary;

        public WebSiteInfo(string webSite, List<WebSitesResultViewModel> webSiteResult)
        {
            InitializeComponent();
            webSites.ItemsSource = webSiteResult;

            this.webSiteResult = webSiteResult;
            this.webSite = webSite;

            ///URL (page header) title:
            lblURL.Text = webSite;
            
            ///lblStatus styling:
            lblStatus.BackgroundColor = Color.FromHex(Android.App.Application.Context.Resources.GetString(Resource.String.allGood));
            lblStatus.Text = "ONLINE";
            int errorCount = webSiteResult.Count(w => w.Status != 3);
            if (webSiteResult.Count(w => w.Status == 3) == webSiteResult.Count)
            {
                lblStatus.BackgroundColor = Color.FromHex(Android.App.Application.Context.Resources.GetString(Resource.String.allGood));
                lblStatus.Text = "ONLINE";
            }
            else if (errorCount < webSiteResult.Count && errorCount > 0)
            {
                lblStatus.BackgroundColor = Color.FromHex(Android.App.Application.Context.Resources.GetString(Resource.String.partialError));
                lblStatus.Text = "PROBLEM";
            }
            else if (errorCount == webSiteResult.Count)
            {
                lblStatus.BackgroundColor = Color.FromHex(Android.App.Application.Context.Resources.GetString(Resource.String.error));
                lblStatus.Text = "OFFLINE";
            }

            ///SLA average logic:
            List<double> AverageItems = new List<double>();
            ICacheService cacheService = new CacheService();
            string sessionId = cacheService.GetCache(Android.App.Application.Context.Resources.GetString(Resource.String.sessionId)).ToString();
            IServices services = new Services.Services();
            JObject slaJson = services.GetSlaList(sessionId, webSiteResult);


            foreach (JProperty DataCenter in slaJson["summary"])
            {
                if (DataCenter.Value.ToString().IndexOf("[]") == -1) {
                    AverageItems.Add(DataCenter.Value["avg_sla"].ToObject<double>());
                    //Console.WriteLine("Data Center: {0}%.", DataCenter.Value["avg_sla"].ToString());
                }

                //SlaViewModel item = null;
                //if (DataCenter.Value.ToString().IndexOf("[]") > -1)
                //{
                //    item = new SlaViewModel()
                //    {
                //        Average = "NA",
                //        Range = "NA",
                //        FromTime = "",
                //        ToTime = "",
                //    };
                //}
                //else
                //{
                //    item = new SlaViewModel()
                //    {
                //        Average = DataCenter.Value["avg_sla"].ToString() + "%",
                //        Range = DataCenter.Value["sla_range"].ToString(),
                //        FromTime = DataCenter.Value["from_time"].ToString(),
                //        ToTime = DataCenter.Value["to_time"].ToString(),
                //    };
                //}

                //slaSummary.Add(DataCenter.Name, item);
            }
            lblSLA.Text = "Daily SLA: " + AverageItems.Average() + "%";
        }

        private void ClickCode(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CodeAnalyze() { Title = "AllProbe" });
        }

        private void ClickSLA(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SLA(webSite, webSiteResult) { Title = "AllProbe" });
        }
    }
}
