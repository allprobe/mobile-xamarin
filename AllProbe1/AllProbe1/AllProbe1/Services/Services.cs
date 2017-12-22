using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using static EventsViewPageViewModel;
using System.Text;
using AllProbe1.ViewModels;
using AllProbe1.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using AllProbe1.Droid;
using System.Threading.Tasks;

namespace AllProbe1.Services
{
    public class Services : IServices
    {
        string error = string.Empty;
        string token = Android.App.Application.Context.Resources.GetString(Resource.String.token);

        public string Login(string email, string password)
        {
            try
            {
                string url = string.Format("https://api.allprobe.com/v2/wLogin/{0}/{1}/none/1/{2}", email, password, token);

                var obj = PostAuthentication(url);
                if (obj == null)
                {
                    return error;
                }
                else
                {
                    var msg = obj["msg"];
                    var sessionId = obj.GetValue("data");
                    var orientation = obj.GetValue("orientation");
                    if (orientation != null && !orientation.ToString().Equals("events"))
                        GlobalServices.setMenuSelected(1);

                    return msg.ToString().Equals("success") ? sessionId.ToString() : null;
                }
            }
            catch (Exception ex)
            {
                return "!Login:" + ex.Message;
            }
        }


        public string LoginFB(string email, string id)
        {
            try
            {
                string url = string.Format("https://api.allprobe.com/v2/wLogin/{0}/{1}/none/2/{2}", email, id, token);

                var obj = PostAuthentication(url);
                if (obj == null)
                {
                    return error;
                }
                else
                {
                    var msg = obj["msg"];
                    var sessionId = obj.GetValue("data");
                    var orientation = obj.GetValue("orientation");
                    if (orientation != null && !orientation.ToString().Equals("events"))
                        GlobalServices.setMenuSelected(1);

                    return msg.ToString().Equals("success") ? sessionId.ToString() : null;
                }
            }
            catch (Exception ex)
            {
                return "!Login:" + ex.Message;
            }
        }

        public async Task<List<JToken>> GetEvents(string sessionId)
        {
            if (sessionId == null)
                return null;
            string url = string.Format("https://api.allprobe.com/v2/GetUserLiveEvents/{0}/{1}", sessionId, token);

            JArray obj = await GetJson(url);
            return obj.ToList();
        }

        public async Task<Dictionary<string, List<WebSitesResultViewModel>>> GetWebSites(string sessionId)
        {
            if (sessionId == null)
                return null;
            string url = string.Format("https://api.allprobe.com/v2/GetUserWebsites/{0}/{1}", sessionId, token);

            JObject obj = await PostJson(url);
            if (obj == null)
                return null;

            Dictionary<string, List<WebSitesResultViewModel>> webSites = new Dictionary<string, List<WebSitesResultViewModel>>();

            JArray urls = (JArray)obj["urls"];
            JArray results = (JArray)obj["results"];
            Dictionary<string, Tuple<int?, int>> shortResults = GetWebSiteResults(results);

            foreach (JToken token in urls.ToList())
            {
                string webSite = token["url"].ToString();
                string runnables = token["runnables"].ToString();
                JArray runnablesArray = JArray.Parse(runnables);


                List<WebSitesResultViewModel> wesSitesResult = new List<WebSitesResultViewModel>();
                foreach (JToken runnable in runnablesArray.ToList())
                {
                    string probeId = runnable.ToString().Split('@')[2];
                    string dataCenter = runnable.ToString().Split('@')[3];
                    Tuple<int?, int> result = GetWebSiteResult(shortResults, probeId);

                    WebSitesResultViewModel webSitesResultViewModel = new WebSitesResultViewModel
                    {
                        Runnable = runnable.ToString(),
                        ProbeId = probeId,
                        Country = dataCenter.Split('_')[0],
                        DataCenter = dataCenter,
                        ResposeTime = result.Item1,
                        Status = result.Item2
                    };

                    wesSitesResult.Add(webSitesResultViewModel);
                }
                byte[] data = Convert.FromBase64String(webSite);
                webSites.Add(Encoding.UTF8.GetString(data), wesSitesResult);
            }

            return webSites;
        }

        private Dictionary<string, Tuple<int?, int>> GetWebSiteResults(JArray results)
        {
            Dictionary<string, Tuple<int?, int>> shortResults = new Dictionary<string, Tuple<int?, int>>();
            foreach (JToken result in results.ToList())
            {
                JArray item = JArray.Parse(result.ToString());
                JArray allResult = JArray.Parse(item[1][0][1].ToString());
                Tuple<int?, int> items = new Tuple<int?, int>(allResult[2] != null ? allResult[2].ToObject<int?>() : null, allResult[4].ToObject<int>());
                shortResults.Add(item[0].ToString().Split('@')[2], items);
            }

            return shortResults;
        }

        private Tuple<int?, int> GetWebSiteResult(Dictionary<string, Tuple<int?, int>> results, string probeId)
        {
            if (results.ContainsKey(probeId))
                return results[probeId];
            else return new Tuple<int?, int>(null, 3);

        }

        public async Task<IList<PageTypeGroup>> GetGroupedEvents(string sessionId)
        {
            try
            {
                if (sessionId == null)
                    return null;
                IList<PageTypeGroup> groupedEvents = new List<PageTypeGroup>();
                List<JToken> all = await GetEvents(sessionId);
                string fixedToken = null;

                foreach (JToken token in all)
                {
                    fixedToken = token.Last.ToString();
                    JObject tkn = JObject.Parse(fixedToken);
                    EventViewModel item = new EventViewModel()
                    {
                        Data = tkn["data"].ToString().PadRight(35, ' '),
                        Event_sub_type = tkn["event_sub_type"].ToString(),
                        Ack = tkn["ack"].ToString(),
                        Host_name = tkn["host_name"].ToString(),
                        Data_center = tkn["prober_id"].ToString(),
                        Result = tkn["result"].ToString().Replace("\n", string.Empty).Replace(" ", "").PadRight(25, ' '),
                        Severity = tkn["severity"].ToString(),
                        Timestamp = tkn["timestamp"].ToString(),
                        Probe_id = token.First.ToString().Split('@')[3]
                    };

                    PageTypeGroup pageTypeGroup = GetHostPage(groupedEvents, item.Host_name);
                    pageTypeGroup.Add(item);
                }

                return groupedEvents;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public JObject GetSlaList(string sessionId, List<WebSitesResultViewModel> webSitesList)
        {
            try
            {
                string url = string.Format("https://api.allprobe.com/v2/GetUserWebsitesSla/{0}/{1}", sessionId, token);
                string jsonObject = GetRunnables(webSitesList);

                var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var result = httpClient.PostAsync(url, content).Result;

                result.EnsureSuccessStatusCode();
                var responseBody = result.Content.ReadAsStringAsync();
                string jsonString = responseBody.Result;
                return JObject.Parse(jsonString);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool PostReport(string email, string webSite, string sessionId)
        {
            string url = string.Format("https://api.allprobe.com/v2/SendApplicationReport/{0}/{1}", sessionId, token);
            var json = new JObject();
            json.Add("email", email);
            json.Add("webSite", webSite);
            var resultJson = new JObject();
            resultJson.Add("report", json);

            string jsonString = resultJson.ToString();

            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = httpClient.PostAsync(url, content).Result;

            result.EnsureSuccessStatusCode();
            var responseBody = result.Content.ReadAsStringAsync();
            string resultString = responseBody.Result;
            return resultString.IndexOf("true") > -1;
        }

        private string GetRunnables(List<WebSitesResultViewModel> webSitesList)
        {
            JObject runnablesJson = new JObject();
            JArray runnablesArray = new JArray();

            foreach (WebSitesResultViewModel webSitesResult in webSitesList)
            {
                string runnable = webSitesResult.Runnable + "@" + webSitesResult.DataCenter;
                runnablesArray.Add(runnable);
            }
            runnablesJson.Add("runnables", runnablesArray);

            return runnablesJson.ToString();
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

        private async Task<JArray> GetJson(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;

                using (WebClient webClient = new WebClient())
                {
                    byte[] arr = await webClient.DownloadDataTaskAsync(uri);
                    string result = Encoding.UTF8.GetString(arr);
                    return JArray.Parse(result);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        private async Task<JObject> PostJson(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;

                using (WebClient webClient = new WebClient())
                {
                    byte[] arr = await webClient.DownloadDataTaskAsync(url);
                    string result = Encoding.UTF8.GetString(arr);
                    return JObject.Parse(result);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        private JObject PostAuthentication(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;

                using (WebClient webClient = new WebClient())
                {
                    byte[] arr = webClient.DownloadData(url);
                    string result = Encoding.UTF8.GetString(arr);
                    return JObject.Parse(result);
                }
            }
            catch (Exception ex)
            {
                error = "!PostAuthentication: " + ex.Message;
                return null;
            }
        }
    }
}
