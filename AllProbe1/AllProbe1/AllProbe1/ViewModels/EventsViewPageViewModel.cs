using AllProbe1.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

public class EventsViewPageViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public class EventViewModel
    {
        public string Timestamp { get; set; }
        public string Data { get; set; }
        public string Ack { get; set; }
        public string Host_name { get; set; }
        public string Severity { get; set; }
        public string Data_center { get; set; }
        public string Event_sub_type { get; set; }
        public string Result { get; set; }
        public string Probe_id { get; set; }

        public string EventName
        {
            get
            {
                if (Data.Length > 20)
                    return Data.Substring(0, 18) + "...";
                return Data;
            }
        }

        private DateTime time
        {
            get
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(Convert.ToInt64(Timestamp) / 1000d)).ToLocalTime();
            }
        }

        public string ElapsedTime
        {
            get
            {
                TimeSpan timeDifference = DateTime.Now.Subtract(time);
                return (timeDifference.Days > 0 ? timeDifference.Days + "d " : "") + timeDifference.Hours.ToString().PadLeft(2, '0') + ":" + timeDifference.Minutes.ToString().PadLeft(2, '0');
            }
        }

        public string ProbeType
        {
            get
            {
                return Probe_id.Split('_')[0];
            }
        }

        public string SeverityColor
        {
            get
            {
                switch (Severity.ToString().ToLower())
                {
                    case "notice":
                        return "#FFD740";
                    case "warning":
                        return "#FFAB40";
                    case "high":
                        return "#FFC400";
                    case "critical":
                        return "#FF6F00";
                    case "disaster":
                        return "#D50000";
                    default:
                        return "white";
                }
            }
        }

        public override string ToString()
        {
            try
            {
                if (Probe_id == null)
                    return Data + "\nHost:  " + Host_name + "\nData center: " + Data_center +
                  "\nResults: " + Regex.Replace(Result, @"\s+", "") +
                  "\nElapsed time: " + ElapsedTime;

                string results = GlobalServices.GetResultString(ProbeType, Result);
                return Data + "\nHost:  " + Host_name + "\nData center: " + Data_center +
                       "\nResults: " + results +
                       "\nElapsed time: " + ElapsedTime;
            }
            catch (Exception)
            {
                return Data + "\nHost:  " + Host_name + "\nData center: " + Data_center +
                  "\nResults: " + Regex.Replace(Result, @"\s+", "") +
                  "\nElapsed time: " + ElapsedTime;
            }
        }
    }

    public class PageTypeGroup : List<EventViewModel>
    {
        public string Title { get; set; }
        public PageTypeGroup(string title)
        {
            Title = title;
        }

        public static IList<PageTypeGroup> All { private set; get; }
    }
}
