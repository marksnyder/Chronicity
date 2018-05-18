using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Provider.InMemory.Data
{
    public class DataBank
    {
        public Dictionary<string,List<TimeAndState>> TrackedState { get; }
        public Dictionary<string, DateTime> LastState { get; }
        public List<StatefulEvent> StatefulEvents { get; }
        public List<string> EventTypes { get; set; }

        public DataBank()
        {
            TrackedState = new Dictionary<string, List<TimeAndState>>();
            LastState = new Dictionary<string, DateTime>();
            StatefulEvents = new List<StatefulEvent>();
            EventTypes = new List<string>();
        }
    }
}
