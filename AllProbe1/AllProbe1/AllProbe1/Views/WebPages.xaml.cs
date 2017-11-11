using AllProbe1.Droid;
using AllProbe1.Models;
using AllProbe1.Services;
using AllProbe1.ViewModels;
using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllProbe1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebPages : ContentPage
    {
        string sessionId = null;
        int intervalInSeconds = 30;
        Dictionary<string, List<WebSitesResultViewModel>> webSites = null;

        public WebPages()
        {
            try
            {
                InitializeComponent();

                ICacheService cacheService = new CacheService();
                sessionId = cacheService.GetCache(Android.App.Application.Context.Resources.GetString(Resource.String.sessionId)).ToString();
                RefreshWebSites();

                Device.StartTimer(TimeSpan.FromSeconds(this.intervalInSeconds), () =>
                {
                    Device.BeginInvokeOnMainThread(() => RefreshWebSites());
                    return true;
                });
            }
            catch (Exception ex)
            {
                webSitesList.IsVisible = false;
                lblError.Text = ex.Message;
                lblError.IsVisible = true;
            }
        }

        private async void RefreshWebSites()
        {
            try
            {
                SessionState sessionState = SessionState.GetInstance();
                ICollection<string> webSitesStringList = sessionState.OldWebSites;

                webSites = GetCachedWebSites();
                if (webSites == null)
                {
                    IServices services = new Services.Services();
                    webSites = await services.GetWebSites(sessionId);
                    if (webSites == null)
                        return;
                }

                webSitesList.ItemsSource = GetWebSitesBindingSource();
                System.Globalization.CultureInfo.CurrentCulture.ClearCachedData();
                this.Title = "Web sites\n" + DateTime.Now.ToShortTimeString();
            }
            catch (Exception ex)
            {
                webSitesList.IsVisible = false;
                lblError.Text = ex.Message;
                lblError.IsVisible = true;
            }
        }

        private List<WebSitesViewModel>  GetWebSitesBindingSource()
        {
            List<WebSitesViewModel> webSitesStatus = new List<WebSitesViewModel>();
            string titleColor = Android.App.Application.Context.Resources.GetString(Resource.String.allGood);

            foreach (string key in webSites.Keys)
            {
                List<WebSitesResultViewModel> webSiteResults = webSites[key];
                string color = Android.App.Application.Context.Resources.GetString(Resource.String.allGood);
                int dataCentersCount = webSiteResults.Count;
                int errorCount = webSiteResults.Count(w => w.Status != 3);

                if (webSiteResults.Count(w => w.Status == 3) == webSiteResults.Count)
                    color = Android.App.Application.Context.Resources.GetString(Resource.String.allGood);
                else if (errorCount < webSiteResults.Count && errorCount > 0)
                {
                    color = Android.App.Application.Context.Resources.GetString(Resource.String.partialError);
                    if (titleColor.Equals(Android.App.Application.Context.Resources.GetString(Resource.String.allGood)))
                        titleColor = Android.App.Application.Context.Resources.GetString(Resource.String.partialError);
                }
                else if (errorCount == webSiteResults.Count)
                {
                    color = Android.App.Application.Context.Resources.GetString(Resource.String.error);
                    titleColor = Android.App.Application.Context.Resources.GetString(Resource.String.error);
                }

                WebSitesViewModel webSite = new WebSitesViewModel()
                {
                    WebSite = key,
                    StatusColor = color
                };

                webSitesStatus.Add(webSite);
                lblColorTitle.BackgroundColor = Color.FromHex(titleColor);
            }

            return webSitesStatus.OrderBy(w => w.WebSite).ToList();
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
           => ((ListView)sender).SelectedItem = null;

        async void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            string webSite = ((WebSitesViewModel)e.SelectedItem).WebSite;
            List<WebSitesResultViewModel> webSiteResult = webSites[webSite];

            await Navigation.PushAsync(new WebSiteInfo(webSite, webSiteResult) { Title="AllProbe" });

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private void ClickSendReport(object sender, EventArgs e)
        {
            string webSite = ((WebSitesViewModel)((Button)sender).BindingContext).WebSite;
            List<WebSitesResultViewModel> webSiteResult = webSites[webSite];

            var popupPage = new EmailPopup(webSite);
            Navigation.PushModalAsync(popupPage);
        }

        public Dictionary<string, List<WebSitesResultViewModel>> GetCachedWebSites()
        {
            string lastWebSitesString = Android.App.Application.Context.Resources.GetString(Resource.String.lastWebSites);
            var contextPref = Android.App.Application.Context.GetSharedPreferences("AllProbe", FileCreationMode.Private);
            ICollection<string> stringValues = contextPref.GetStringSet(lastWebSitesString, null);

            ICacheService cacheService = new CacheService();
            return cacheService.GetCachedWebSite(stringValues);
        }
    }
}
