using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core.Events
{
    public class Event
    {
        public string Type { get; set; }
        public string On { get; set; }
        public string Entity { get; set; }
        public IEnumerable<string> Observations { get; set; }
    }
}
