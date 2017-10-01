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
        public int Status { get; set; }

        public string StatusColor
        {
            get
            {
                switch (Status)
                {
                    case 1:
                        return Android.App.Application.Context.Resources.GetString(Resource.String.error);
                    case 2:
                        return Android.App.Application.Context.Resources.GetString(Resource.String.partialError);
                    case 3:
                        return Android.App.Application.Context.Resources.GetString(Resource.String.allGood);
                    default:
                        return Android.App.Application.Context.Resources.GetString(Resource.String.allGood);
                }

            }
        }
    }
}
