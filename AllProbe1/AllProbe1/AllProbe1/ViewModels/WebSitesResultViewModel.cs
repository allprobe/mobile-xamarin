using AllProbe1.Droid;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllProbe1.ViewModels
{
    public class WebSitesResultViewModel
    {
        public string Runnable { get; set; }
        public string ProbeId { get; set; }
        public string DataCenter { get; set; }
        public string Country { get; set; }
        public int? ResposeTime { get; set; }
        public double SLAaverage {get; set;}
        public int Status { get; set; }
        public List<SlaViewModel> WebSiteIssues { get; set; }
        public bool ShowAtView { get; set; }

        public string ResponseTimeString
        {
            get
            {
                if(ResposeTime>1000)
                    return string.Format("{0:0.00} sec", (double)ResposeTime/1000);
                else
                    return string.Format("{0} ms", ResposeTime);
            }
        }

        public string SLAString
        {
            get
            {
                return string.Format("{0}%", SLAaverage);
            }
        }

        public string StatusColor
        {
            get
            {
                switch (Status)
                {
                    case 1:
                        return Android.App.Application.Context.Resources.GetString(Resource.Color.error);
                    case 2:
                        return Android.App.Application.Context.Resources.GetString(Resource.Color.partialError);
                    case 3:
                        return Android.App.Application.Context.Resources.GetString(Resource.Color.allGood);
                    default:
                        return Android.App.Application.Context.Resources.GetString(Resource.Color.allGood);
                }

            }
        }
    }
}
