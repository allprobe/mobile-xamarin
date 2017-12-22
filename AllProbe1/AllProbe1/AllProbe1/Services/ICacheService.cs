using AllProbe1.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using static EventsViewPageViewModel;

namespace AllProbe1.Services
{
    internal interface ICacheService
    {
        void SetCachedWeSites(string key, Dictionary<string, List<WebSitesResultViewModel>> values);
        Dictionary<string, List<WebSitesResultViewModel>> GetCachedWebSite(ICollection<string> collection);
        ICollection<string> GetStringWebSites(Dictionary<string, List<WebSitesResultViewModel>> values);
        void SetCache(string key, string value);
        string GetCache(string key);
        IList<PageTypeGroup> GetCachedEvents();
    }
}
