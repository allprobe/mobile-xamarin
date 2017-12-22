using AllProbe1.Droid;
using AllProbe1.Models;
using AllProbe1.ViewModels;
using Android.App;
using Android.Support.V4.App;
using Android.Content;
using Android.Graphics;
using Java.Lang;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using static EventsViewPageViewModel;
using AllProbe1.Services;
using Android.OS;

namespace AllProbe1.Services
{
    [BroadcastReceiver]
    [IntentFilter(new string[] { Intent.ActionBootCompleted }, Priority = Int32.MaxValue)]
    public class AlarmReceiver : BroadcastReceiver
    {
        private string seperator = Application.Context.Resources.GetString(Resource.String.stringSeperator);
        private SessionState sessionState = SessionState.GetInstance();
        private ICacheService cacheService = new CacheService();
        private IServices services = new Services();

        public override void OnReceive(Context context, Intent intent)
        {
            //TriggerAlert(context, intent);
            cacheService.SetCache(Android.App.Application.Context.Resources.GetString(Resource.String.isAlarmManagerRunnning), "true");

            Handler handler = new Handler();
            Action callback = null;
            callback = () =>
            {
                TriggerAlert(context, intent);
                handler.PostDelayed(callback, 30000);
            };
            handler.PostDelayed(callback, 0);
        }

        private void TriggerAlert(Context context, Intent intent)
        {
            GetEvents();
            GetWebSites();
        }

        private async void GetEvents()
        {
            try
            {
                if (string.IsNullOrEmpty(sessionState.SessionId))
                {
                    sessionState.SessionId = GetCache(Application.Context.Resources.GetString(Resource.String.sessionId));
                }

                if (sessionState.SessionId == null)
                    return;

                List<JToken> allvents = await services.GetEvents(sessionState.SessionId);

                List<EventViewModel> newEvents = new List<EventViewModel>();
                foreach (JToken token in allvents)
                {
                    string fixedToken = token.Last.ToString();
                    JObject tkn = JObject.Parse(fixedToken);

                    // If severity is in range
                    object severity = null;
                    if (Xamarin.Forms.Application.Current.Properties.ContainsKey(Android.App.Application.Context.Resources.GetString(Resource.String.selectedSeverity)))
                        severity = Xamarin.Forms.Application.Current.Properties[Android.App.Application.Context.Resources.GetString(Resource.String.selectedSeverity)];
                    if (severity == null || (severity != null && GlobalServices.IsSeveritiesFeet(tkn["severity"].ToString(), Convert.ToInt32(severity))))
                    {
                        EventViewModel item = new EventViewModel()
                        {
                            Data = tkn["data"].ToString(),
                            Event_sub_type = tkn["event_sub_type"].ToString(),
                            Ack = tkn["ack"].ToString(),
                            Host_name = tkn["host_name"].ToString(),
                            Data_center = tkn["prober_id"].ToString(),
                            Result = tkn["result"].ToString().Replace("\n", string.Empty).Replace(" ", ""),
                            Severity = tkn["severity"].ToString(),
                            Timestamp = tkn["timestamp"].ToString(),
                            Probe_id = token.First.ToString().Split('@')[3]
                        };

                        newEvents.Add(item);
                    }
                }

                EventNotifications(newEvents);
            }
            catch (System.Exception ex)
            {
                CreateNotification("AllProbe", ex.Message, 10, 0);
            }
        }

        private void EventNotifications(List<EventViewModel> newEvents)
        {
            if (sessionState.OldEvents == null)
            {
                sessionState.OldEvents = newEvents.Select(e => e.Timestamp + e.Data_center + e.Data).ToList();
                return;
            }

            var intersectedEvents = sessionState.OldEvents.OfType<string>().Intersect(newEvents.Select(n => n.Timestamp + n.Data_center + n.Data));

            SetCachedEvents(Application.Context.Resources.GetString(Resource.String.lastEvents), newEvents);
            if (newEvents.Count() > intersectedEvents.Count() && sessionState.OldEvents.Count() > intersectedEvents.Count())
            {
                int newEventsCount = newEvents.Count() - intersectedEvents.Count() + sessionState.OldEvents.Count() - intersectedEvents.Count();
                CreateNotification("Events changed", "New Events were detected and old events were canceled", 10, newEventsCount);
            }
            else if (newEvents.Count() > intersectedEvents.Count())
            {
                int newEventsCount = newEvents.Count() - intersectedEvents.Count();
                CreateNotification(newEventsCount.ToString() + (newEventsCount == 1 ? " New event" : " New events"), "New Events were detected", 10, newEventsCount);
            }
            else if (sessionState.OldEvents.Count() > intersectedEvents.Count())
            {
                int newEventsCount = sessionState.OldEvents.Count() - intersectedEvents.Count();
                CreateNotification(newEventsCount.ToString() + (newEventsCount == 1 ? " Event was canceled" : " Events were canceled"), "Events were canceled", 10, newEventsCount);
            }

            sessionState.OldEvents = newEvents.Select(e => e.Timestamp + e.Data_center + e.Data).ToList();
        }

        private async void GetWebSites()
        {
            try
            {
                if (string.IsNullOrEmpty(sessionState.SessionId))
                {
                    sessionState.SessionId = GetCache(Application.Context.Resources.GetString(Resource.String.sessionId));
                }

                if (sessionState.SessionId == null)
                    return;

                Dictionary<string, List<WebSitesResultViewModel>> webSites = await services.GetWebSites(sessionState.SessionId);
                cacheService.SetCachedWeSites(Application.Context.Resources.GetString(Resource.String.lastWebSites), webSites);

                if (sessionState.OldWebSites == null)
                {
                    sessionState.OldWebSites = cacheService.GetStringWebSites(webSites);
                    return;
                }

                ICollection<string> newWebSiteStrings = cacheService.GetStringWebSites(webSites);

                var notIntersected = GetNotIntersection(newWebSiteStrings, sessionState.OldWebSites);
                int notIntersectedCount = notIntersected.Count();
                if (notIntersectedCount > 0)
                {
                    //int newWebSitesCountCount = newWebSiteStrings.Count() - intersection + oldWebSites.Count() - intersection;
                    CreateNotification("Web site alert", "Check the status of the web sites", 10, notIntersectedCount);
                }
                //else if (newWebSiteStrings.Count() > intersection)
                //{
                //    int newWebSitesCountCount = newWebSiteStrings.Count() - intersection;
                //    CreateNotification(newWebSitesCountCount.ToString() + (newWebSitesCountCount == 1 ? " Web site error" : " Web sites error"), "New web site errors were detected", 10, newWebSitesCountCount);
                //}
                //else if (oldWebSites.Count() > intersection)
                //{
                //    int newWebSitesCountCount = oldWebSites.Count() - intersection;
                //    CreateNotification(newWebSitesCountCount.ToString() + (newWebSitesCountCount == 1 ? " Web site was fixed" : " Web site was fixed"), "Web site error was fixed", 10, newWebSitesCountCount);
                //}

                sessionState.OldWebSites = newWebSiteStrings;
            }
            catch (System.Exception)
            {

            }
        }

        private IEnumerable<string> GetNotIntersection(ICollection<string> newWebSites, ICollection<string> oldWebSites)
        {
            var newWeb = newWebSites.Select(n => n.Split(Convert.ToChar(seperator))[0] + seperator + n.Split(Convert.ToChar(seperator))[6]).ToList();
            var oldWeb = oldWebSites.Select(n => n.Split(Convert.ToChar(seperator))[0] + seperator + n.Split(Convert.ToChar(seperator))[6]).ToList();
            var intersect = newWeb.Union(oldWeb).Except(newWeb.Intersect(oldWeb));

            return intersect;
            //return newWebSites.Select(n => n.Split(Convert.ToChar(seperator))[0] + seperator + n.Split(Convert.ToChar(seperator))[6]).OfType<string>().Intersect(oldWebSites.Select(n => n.Split(Convert.ToChar(seperator))[0] + seperator + n.Split(Convert.ToChar(seperator))[6])).Count();
        }

        private void SetCache(string key, string value)
        {
            var contextPref = Application.Context.GetSharedPreferences("AllProbe", FileCreationMode.Private);
            var contextEdit = contextPref.Edit();
            contextEdit.PutString(key, value);
            contextEdit.Commit();
        }

        private string GetCache(string key)
        {
            //var contextPref = Application.Context.GetSharedPreferences("AllProbe", FileCreationMode.Private);
            return cacheService.GetCache(key);
        }

        private void CreateNotification(string title, string description, int anId, int number)
        {
            Intent intent = new Intent(Application.Context, typeof(MainActivity));

            /// Create a PendingIntent; we're only using one PendingIntent (ID = 0):
            const int pendingIntentId = 0;
            PendingIntent pendingIntent =
                PendingIntent.GetActivity(Application.Context, pendingIntentId, intent, PendingIntentFlags.OneShot);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(Application.Context, "DEFAULT_CHANNEL_ID")
                            .SetContentIntent(pendingIntent)
                            .SetContentTitle(title)
                            .SetContentText(description)
                            .SetDefaults(NotificationCompat.DefaultAll)
                            .SetSmallIcon(Resource.Drawable.allProbe)
                            .SetColor(Convert.ToInt32(0x00FFFFFF))//Background notification icon color = Transparent
                            .SetOnlyAlertOnce(true)
                            .SetWhen(JavaSystem.CurrentTimeMillis())
                            //.SetLargeIcon(BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.allProbe))
                            .SetShowWhen(true)
                            .SetAutoCancel(true)
                            .SetNumber(number)
                            .SetBadgeIconType(NotificationCompat.BadgeIconNone);


            /// Build the notification:
            Notification notification = builder.Build();

            /// Get the notification manager:
            NotificationManager notificationManager =
                Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

            /// Publish the notification:
            const int notificationId = 1095;
            notificationManager.Notify(notificationId, notification);
        }

        private void SetCachedEvents(string key, List<EventViewModel> values)
        {
            ICollection<string> stringValues = values.Select(e => e.Timestamp + seperator + e.Data + seperator + e.Ack + seperator +
                               e.Host_name + seperator + e.Severity + seperator + e.Data_center + seperator + e.Event_sub_type + 
                               seperator + e.Result + seperator + e.Probe_id).ToList();

            var contextPref = Application.Context.GetSharedPreferences("AllProbe", FileCreationMode.Private);
            var contextEdit = contextPref.Edit();
            contextEdit.PutStringSet(key, stringValues);
            contextEdit.Commit();
        }

        private ICollection<string> GetCacheList(string key)
        {
            try
            {
                var contextPref = Application.Context.GetSharedPreferences("AllProbe", FileCreationMode.Private);
                return contextPref.GetStringSet(key, null).OrderBy(e => e).ToList();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public bool AppInForeground()
        {
            Context context = Application.Context;
            ActivityManager activityManager = (ActivityManager)context.GetSystemService(Context.ActivityService);
            IList<ActivityManager.RunningAppProcessInfo> appProcesses = activityManager.RunningAppProcesses;
            if (appProcesses == null)
            {
                return false;
            }
            string packageName = context.PackageName;
            foreach (ActivityManager.RunningAppProcessInfo appProcess in appProcesses)
            {
                if (appProcess.Importance == Importance.Foreground && appProcess.ProcessName == packageName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
