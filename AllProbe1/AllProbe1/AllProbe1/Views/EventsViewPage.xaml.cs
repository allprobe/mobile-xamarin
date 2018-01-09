using AllProbe1.Droid;
using AllProbe1.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static EventsViewPageViewModel;

namespace AllProbe1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventsViewPage : ContentPage
    {
        ICacheService cacheService = new CacheService();
        IList<PageTypeGroup> events = null;
        private string sessionId = null;
        private int intervalInSeconds = 30;

        public EventsViewPage()
        {
            try
            {
                InitializeComponent();

                this.sessionId = cacheService.GetCache(Android.App.Application.Context.Resources.GetString(Resource.String.sessionId)).ToString();

                RefreshEvents();

                Device.StartTimer(TimeSpan.FromSeconds(this.intervalInSeconds), () =>
                {
                    Device.BeginInvokeOnMainThread(() => RefreshEvents());
                    return true;
                });
            }
            catch (Exception ex)
            {
                eventsList.IsVisible = false;
                //lblErrorEvents.Text = ex.Message;
                lblErrorEvents.IsVisible = true;
            }
        }

        private async void RefreshEvents()
        {
            try
            {
                ///Initialize messages' visibility 
                eventsList.IsVisible = true;
                ZeroEvents.IsVisible = false;
                lblErrorEvents.IsVisible = false;
                ZeroWebSites.IsVisible = false;

                events = cacheService.GetCachedEvents();
                if (events == null)
                {
                    IServices services = new Services.Services();
                    events = await services.GetGroupedEvents(sessionId);

                    if (events == null)
                        return;
                    //if (eventsList.ItemsSource != null && events != null && events.Count > 0)
                    //    NotifyOnChanged(eventsList.ItemsSource, events);
                }

                eventsList.ItemsSource = events;
                if (events.Count == 0)
                {
                    eventsList.IsVisible = false;
                    if (GlobalServices.GetOrientation() == 0) //user has 0 evets because he is none
                        ZeroWebSites.IsVisible = true;
                    else    //user is !none and has 0 events
                        ZeroEvents.IsVisible = true;
                }
                lblUdpated.Text = "Updated\n" + DateTime.Now.ToShortTimeString();
                //this.Title = "Events\n" + DateTime.Now.ToShortTimeString();
            }
            catch (Exception ex)
            {
                eventsList.IsVisible = false;
                //lblErrorEvents.Text = ex.Message;
                lblErrorEvents.IsVisible = true;
            }
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
            => ((ListView)sender).SelectedItem = null;

        async void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var selectedItem = ((EventViewModel)e.SelectedItem);
            await DisplayAlert(selectedItem.Severity + " " + selectedItem.ProbeType + " Alert", e.SelectedItem.ToString(), "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private void NotifyOnChanged(IEnumerable oldEvents, IList<PageTypeGroup> newEvents)
        {
        }
    }

}
