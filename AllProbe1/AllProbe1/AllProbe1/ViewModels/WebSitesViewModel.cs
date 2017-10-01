using System;
using System.Collections.Generic;
using System.Text;

namespace AllProbe1.ViewModels
{
    public class WebSitesViewModel
    {
        public string WebSite { get; set; }
        public string StatusColor { get; set; }
        public string WebSiteShort {
            get
            {
                if (WebSite.Length > 30)
                    return WebSite.Substring(0, 30) + "...";
                return WebSite;
            }
        }
    }
}
