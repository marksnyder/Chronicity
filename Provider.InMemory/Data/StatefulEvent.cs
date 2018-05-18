using Chronicity.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Provider.InMemory.Data
{
    public class StatefulEvent
    {
        public Event Event { get; }
        public IEnumerable<TimeAndState> TimeAndStates { get; }

        public StatefulEvent(Event e, IEnumerable<TimeAndState> s)
        {
            Event = e;
            TimeAndStates = s;
        }
    }
}
