using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core.Events
{
    public class Context
    {
        public Event Event { get; set; }
        public Dictionary<string,string> State { get; set; }
        public List<string> Links { get; set; }
    }
}
