using AllProbe1.Droid;
using AllProbe1.Models;
using AllProbe1.ViewModels;
using Android.App;
using Android.Content;
using System;
using System.Collections.Generic;
using System.Text;
using static EventsViewPageViewModel;

namespace AllProbe1.Services
{
    public class CacheService : ICacheService
    {
        private string seperator = Application.Context.Resources.GetString(Resource.String.stringSeperator);

        public void SetCachedWeSites(string key, Dictionary<string, List<WebSitesResultViewModel>> values)
        {
            ICollection<string> stringValues = GetStringWebSites(values);

            var contextPref = Application.Context.GetSharedPreferences("AllProbe", FileCreationMode.Private);
            var contextEdit = contextPref.Edit();
            contextEdit.PutStringSet(key, stringValues);
            contextEdit.Commit();
        }

        public ICollection<string> GetStringWebSites(Dictionary<string, List<WebSitesResultViewModel>> values)
        {
            ICollection<string> stringValues = new List<string>();
            foreach (string vKey in values.Keys)
            {
                foreach (WebSitesResultViewModel webSite in values[vKey])
                {
                    stringValues.Add(vKey + seperator + webSite.Country + seperator + webSite.DataCenter + seperator + webSite.ProbeId + seperator +
                                            webSite.ResposeTime + seperator + webSite.Runnable + seperator + webSite.Status);
                }
            }

            return stringValues;
        }

        public Dictionary<string, List<WebSitesResultViewModel>> GetCachedWebSite(ICollection<string> collection)
        {
            Dictionary<string, List<WebSitesResultViewModel>> webSites = new Dictionary<string, List<WebSitesResultViewModel>>();

            foreach (string item in collection)
            {
                string[] splitString = item.Split(Convert.ToChar(seperator));
                int? responseTime = null;

                if (!webSites.ContainsKey(splitString[0]))
                    webSites.Add(splitString[0], new List<WebSitesResultViewModel>());

                if (!string.IsNullOrEmpty(splitString[4]))
                    responseTime = Convert.ToInt32(splitString[4]);

                WebSitesResultViewModel webSite = new WebSitesResultViewModel
                {
                    Country = splitString[1],
                    DataCenter = splitString[2],
                    ProbeId = splitString[3],
                    ResposeTime = responseTime,
                    Runnable = splitString[5],
                    Status = Convert.ToInt32(splitString[6]),
                };

                webSites[splitString[0]].Add(webSite);
            }

            return webSites;
        }

        public void SetCache(string key, string value)
        {
            var contextPref = Application.Context.GetSharedPreferences("AllProbe", FileCreationMode.Private);
            
            //if (contextPref.Contains(key))
            //{
                var contextEdit = contextPref.Edit();
                contextEdit.PutString(key, value);
                contextEdit.Commit();
            //}
        }

        public string GetCache(string key)
        {
            var contextPref = Application.Context.GetSharedPreferences("AllProbe", FileCreationMode.Private);
            if (contextPref.Contains(key))
                return contextPref.GetString(key, null);
            return null;
        }

        public IList<PageTypeGroup> GetCachedEvents()
        {
            try
            {
                string lastEventsString = Android.App.Application.Context.Resources.GetString(Resource.String.lastEvents);
                var contextPref = Android.App.Application.Context.GetSharedPreferences("AllProbe", FileCreationMode.Private);
                ICollection<string> stringValues = contextPref.GetStringSet(lastEventsString, null);
                IList<PageTypeGroup> groupedEvents = new List<PageTypeGroup>();
                List<EventViewModel> events = new List<EventViewModel>();

                foreach (string st in stringValues)
                {

                    string[] splittedString = st.Split(Convert.ToChar(Android.App.Application.Context.Resources.GetString(Resource.String.stringSeperator)));
                    object severity = null;
                    if (Xamarin.Forms.Application.Current.Properties.ContainsKey(Android.App.Application.Context.Resources.GetString(Resource.String.selectedSeverity)))
                        severity = Xamarin.Forms.Application.Current.Properties[Android.App.Application.Context.Resources.GetString(Resource.String.selectedSeverity)];
                    if (severity == null || (severity != null && GlobalServices.IsSeveritiesFeet(splittedString[4].ToString(), Convert.ToInt32(severity))))
                    {
                        EventViewModel eventVm = new EventViewModel()
                        {
                            Timestamp = splittedString[0],
                            Data = splittedString[1],
                            Ack = splittedString[2],
                            Host_name = splittedString[3],
                            Severity = splittedString[4],
                            Data_center = splittedString[5],
                            Event_sub_type = splittedString[6],
                            Result = splittedString[7],
                            Probe_id = splittedString.Length == 9? splittedString[8] : null
                        };
                        events.Add(eventVm);

                        PageTypeGroup pageTypeGroup = GetHostPage(groupedEvents, eventVm.Host_name);
                        pageTypeGroup.Add(eventVm);
                    }
                }

                return groupedEvents;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private PageTypeGroup GetHostPage(IList<PageTypeGroup> groupedEvents, string hostName)
        {
            foreach (PageTypeGroup pageTypeGroup in groupedEvents)
            {
                if (pageTypeGroup.Title.Equals(hostName))
                    return pageTypeGroup;
            }

            PageTypeGroup pageTypeGroup1 = new PageTypeGroup(hostName);
            groupedEvents.Add(pageTypeGroup1);
            return pageTypeGroup1;
        }
    }
}
