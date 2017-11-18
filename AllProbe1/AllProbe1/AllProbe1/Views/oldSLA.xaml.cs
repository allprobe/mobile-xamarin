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
    public partial class oldSLA : ContentPage
    {
        string sessionId;
        private string webSite;
        private IList<WebSitesResultViewModel> webSitesList;
        private Dictionary<string, List<SlaViewModel>> issues;
        private Dictionary<string, SlaViewModel> slaSummary;
        private IList<SlaViewModel> slaSelected;

        public oldSLA(string webSite, List<WebSitesResultViewModel> webSitesList)
        {
            InitializeComponent();
            ICacheService cacheService = new CacheService();
            sessionId = cacheService.GetCache(Android.App.Application.Context.Resources.GetString(Resource.String.sessionId)).ToString();

            this.webSitesList = webSitesList;
            this.webSite = webSite;

            lblWebSite.Text = webSite;
            webSites.ItemsSource = webSitesList;

            IServices services = new Services.Services();
            JObject slaJson = services.GetSlaList(sessionId, webSitesList);
            slaSummary = new Dictionary<string, SlaViewModel>();
            issues = new Dictionary<string, List<SlaViewModel>>();

            foreach (JProperty prop in slaJson["summary"])
            {
                SlaViewModel item = null;
                if (prop.Value.ToString().IndexOf("[]") > -1)
                {
                    item = new SlaViewModel()
                    {
                        Average = "NA",
                        Range = "NA",
                        FromTime = "",
                        ToTime = "",
                    };
                }
                else
                {
                    item = new SlaViewModel()
                    {
                        Average = prop.Value["avg_sla"].ToString() + "%",
                        Range = prop.Value["sla_range"].ToString(),
                        FromTime = prop.Value["from_time"].ToString(),
                        ToTime = prop.Value["to_time"].ToString(),
                    };
                }

                slaSummary.Add(prop.Name, item);
            }

            foreach (JProperty prop in slaJson["issues"])
            {
                List<SlaViewModel> slas = new List<SlaViewModel>();
                foreach (JToken token in prop.Value)
                {
                    SlaViewModel item = new SlaViewModel()
                    {
                        Average = token["sla"].ToString() + "%",
                        FromTime = token["from"].ToString(),
                        ToTime = token["to"].ToString(),
                    };
                    slas.Add(item);
                }


                issues.Add(prop.Name, slas);
            }

            SelectDataCenter(webSitesList[0].DataCenter);
        }

        public oldSLA(string webSite, IList<WebSitesResultViewModel> webSitesList)
        {
            this.webSite = webSite;
            this.webSitesList = webSitesList;
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
           => ((ListView)sender).SelectedItem = null;

        async void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var selectedItem = (WebSitesResultViewModel)e.SelectedItem;

            string dataCenter = selectedItem.DataCenter;
            SelectDataCenter(dataCenter);

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private void SelectDataCenter(string dataCenter)
        {
            slaSelected = issues[dataCenter];
            lvSlaSelected.ItemsSource = slaSelected;

            SlaViewModel summary = slaSummary[dataCenter];
            lblSlaSummaryTitle.Text = "SLA(Daily): " + summary.Average;
        }

        private void ClickSendReport(object sender, EventArgs e)
        {
            string webSite = lblWebSite.Text;
            var popupPage = new EmailPopup(webSite);

            Navigation.PushModalAsync(popupPage);
        }
    }
}
