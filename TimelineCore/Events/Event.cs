using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core.Events
{
    public class Event
    {
        public string Type { get; set; }
        public string On { get; set; }
        public IList<string> Entities { get; set; }
        public string Id { get; set; }
    }
}
