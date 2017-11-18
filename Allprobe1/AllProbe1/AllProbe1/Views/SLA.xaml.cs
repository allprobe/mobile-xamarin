using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Newtonsoft.Json.Linq;

using AllProbe1.ViewModels;
using AllProbe1.Services;
using AllProbe1.Droid;


namespace AllProbe1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SLA : ContentPage
    {
        private List<WebSitesResultViewModel> List2show;
        public SLA(string webSite, List<WebSitesResultViewModel> webSiteResult)
        {
            InitializeComponent();

            ///URL (page header) title:
            lblURL.Text = webSite;

            ///SLA average logic:
            List<double> AverageItems = new List<double>();
            ICacheService cacheService = new CacheService();
            string sessionId = cacheService.GetCache(Android.App.Application.Context.Resources.GetString(Resource.String.sessionId)).ToString();
            IServices services = new Services.Services();
            JObject slaJson = services.GetSlaList(sessionId, webSiteResult);

            foreach (JProperty DataCenter in slaJson["summary"])
            {
                if (DataCenter.Value.ToString().IndexOf("[]") == -1)
                {
                    AverageItems.Add(DataCenter.Value["avg_sla"].ToObject<double>());
                }
            }
            lblSLA.Text = "Daily SLA: " + AverageItems.Average() + "%";

            ///Deteailed SLA logic:
            foreach (WebSitesResultViewModel DataCenter in webSiteResult)
            {
                foreach (JProperty DataCenterSummery in slaJson["summary"])
                {
                    if (DataCenterSummery.Name == DataCenter.DataCenter)
                    {
                        DataCenter.SLAaverage = DataCenterSummery.Value["avg_sla"].ToObject<double>();
                    }
                }
                DataCenter.WebSiteIssues = new List<SlaViewModel>();
                foreach (JProperty IssuedDataCenter in slaJson["issues"])
                {
                    if (IssuedDataCenter.Name == DataCenter.DataCenter)
                    {
                        if (IssuedDataCenter.Value.ToString() == "[]")
                        {
                            SlaViewModel IssueItem = new SlaViewModel()
                            {
                                FromTime = "No",
                                ToTime = "issues",
                                Average = "found",
                            };
                            DataCenter.WebSiteIssues.Add(IssueItem);
                        }
                        else
                            foreach (JToken issue in IssuedDataCenter.Value)
                            {
                                SlaViewModel IssueItem = new SlaViewModel()
                                {
                                    Average = string.Format("{0:0.000%}", issue["sla"]),
                                    FromTime = issue["from"].ToString(),
                                    ToTime = issue["to"].ToString(),
                                };
                                DataCenter.WebSiteIssues.Add(IssueItem);
                            }
                    }
                }
                ///SLAdetails default view:
                DataCenter.ShowAtView = true;
            }
            List2show = webSiteResult;
            webSites.ItemsSource = List2show;
        }

        private void ExpandAndCollapseDetails(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            List2show[List2show.IndexOf((WebSitesResultViewModel)e.SelectedItem)].ShowAtView = false;
            ///This is the same:
            //((WebSitesResultViewModel)e.SelectedItem).ShowAtView = false;

            ///This does nothing :(
            webSites.ItemsSource = List2show;

            ///This does nothing either :(
            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    webSites.ItemsSource = null;
            //    webSites.ItemsSource = List2show;
            //});

            ///Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

    }
}