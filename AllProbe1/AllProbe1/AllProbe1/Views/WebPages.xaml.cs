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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AllProbe1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebPages : ContentPage
    {
        string sessionId = null;
        int intervalInSeconds = 30;
        int FirstTimeDelay = 10;
        Dictionary<string, List<WebSitesResultViewModel>> webSites = null;

        public WebPages()
        {
            try
            {
                InitializeComponent();

                ICacheService cacheService = new CacheService();
                sessionId = cacheService.GetCache(Android.App.Application.Context.Resources.GetString(Resource.String.sessionId)).ToString();

                ProgressAndCloseBar();
                //RefreshWebSites();
                Device.StartTimer(TimeSpan.FromSeconds(this.FirstTimeDelay), () =>
                {
                    Device.BeginInvokeOnMainThread(() => RefreshWebSites());
                    return false;
                });
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

        private async void ProgressAndCloseBar()
        {
            await thinking.ProgressTo(1, (uint)FirstTimeDelay * 1000, Easing.Linear);
            thinking.IsVisible = false;
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
                lblUdpated.Text = "Updated\n" + DateTime.Now.ToShortTimeString();
                //this.Title = "Web sites\n" + DateTime.Now.ToShortTimeString();
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
            string titleColor = Android.App.Application.Context.Resources.GetString(Resource.Color.allGood);

            foreach (string key in webSites.Keys)
            {
                List<WebSitesResultViewModel> webSiteResults = webSites[key];
                string color = Android.App.Application.Context.Resources.GetString(Resource.Color.allGood);
                int dataCentersCount = webSiteResults.Count;
                int errorCount = webSiteResults.Count(w => w.Status != 3);

                if (webSiteResults.Count(w => w.Status == 3) == webSiteResults.Count)
                    color = Android.App.Application.Context.Resources.GetString(Resource.Color.allGood);
                else if (errorCount < webSiteResults.Count && errorCount > 0)
                {
                    color = Android.App.Application.Context.Resources.GetString(Resource.Color.partialError);
                    if (titleColor.Equals(Android.App.Application.Context.Resources.GetString(Resource.Color.allGood)))
                        titleColor = Android.App.Application.Context.Resources.GetString(Resource.Color.partialError);
                }
                else if (errorCount == webSiteResults.Count)
                {
                    color = Android.App.Application.Context.Resources.GetString(Resource.Color.error);
                    titleColor = Android.App.Application.Context.Resources.GetString(Resource.Color.error);
                }

                WebSitesViewModel webSite = new WebSitesViewModel()
                {
                    WebSite = key,
                    StatusColor = color
                };

                webSitesStatus.Add(webSite);
                lblColorTitle.BackgroundColor = Color.FromHex(titleColor);
            }
            if (webSitesStatus.Count == 0)
            {
                ZeroWebSites.IsVisible = true;
                webSitesList.IsVisible = false;

            }
            return webSitesStatus.OrderBy(w => w.WebSite).ToList();
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
           => ((ListView)sender).SelectedItem = null;

        void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            string webSite = ((WebSitesViewModel)e.SelectedItem).WebSite;
            List<WebSitesResultViewModel> webSiteResult = webSites[webSite];

            ///Changed navigation type: Instead of pushing NavigationPage, adding a Tab.
            ///If you unchange it, this func should be "async" and see also the comment in MainPage.cs
            //await Navigation.PushAsync(new WebSiteInfo(webSite, webSiteResult));
            var parentPage = this.Parent as TabbedPage;
            var websitepage = new WebSiteInfo(webSite, webSiteResult);
            parentPage.Children.Add(websitepage);
            parentPage.CurrentPage = websitepage;
            
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
