using Chronicity.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Provider.InMemory.Data
{
    public class StatefulEvent
    {
        public Event Event { get; }
        public TimeAndState TimeAndState { get; }

        public StatefulEvent(Event e, TimeAndState s)
        {
            Event = e;
            TimeAndState = s;
        }
    }
}
