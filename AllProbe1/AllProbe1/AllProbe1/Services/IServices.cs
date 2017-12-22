using AllProbe1.ViewModels;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using static EventsViewPageViewModel;

namespace AllProbe1.Services
{
    public interface IServices
    {
        string Login(string email, string password);
        string LoginFB(string email, string id);
        Task<IList<PageTypeGroup>> GetGroupedEvents(string sessionId);
        Task<List<JToken>> GetEvents(string sessionId);
        Task<Dictionary<string, List<WebSitesResultViewModel>>> GetWebSites(string sessionId);
        JObject GetSlaList(string sessionId, List<WebSitesResultViewModel> webSitesList);
        bool PostReport(string email, string webSite, string sessionId);
    }
}
