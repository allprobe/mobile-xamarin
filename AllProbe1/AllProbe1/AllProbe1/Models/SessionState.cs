using System;
using System.Collections.Generic;
using System.Text;
using static AllProbe1.Views.EventsViewPageViewModel;

namespace AllProbe1.Models
{
    public class SessionState
    {
        public string SessionId { get; set; }
        public ICollection<string> OldEvents { get; set; }
        public ICollection<string> OldWebSites { get; set; }
        private static SessionState sessionState = new SessionState();

        private SessionState() { }

        public static SessionState GetInstance() {
            return sessionState;
        }
    }
}
