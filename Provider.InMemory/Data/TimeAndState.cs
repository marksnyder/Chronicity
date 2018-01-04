using Chronicity.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Provider.InMemory.Data
{
    public class TimeAndState
    {
        public EntityState State { get; set; }
        public List<string> Links { get; set; }
        public DateTime On { get; set; }
    }
}
