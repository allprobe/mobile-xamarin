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
                string ShortSiteName=WebSite;

                if (ShortSiteName.Substring(0, 11) == "http://www.")
                    ShortSiteName = WebSite.Substring(11);
                else if (ShortSiteName.Substring(0, 12) == "https://www.")
                    ShortSiteName = WebSite.Substring(12);
                else if (ShortSiteName.Substring(0, 7) == "http://")
                    ShortSiteName = WebSite.Substring(7);
                else if (ShortSiteName.Substring(0, 8) == "https://")
                    ShortSiteName = WebSite.Substring(8);

                if (ShortSiteName.Length > 20)
                    ShortSiteName= ShortSiteName.Substring(0, 20) + "...";

                ///Original code:
                //if (WebSite.Length > 30)
                //    return WebSite.Substring(0, 30) + "...";

                return ShortSiteName;
            }
        }
    }
}
