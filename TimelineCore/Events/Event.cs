using Chronicity.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core.Events
{
    public class Event
    {
        public string Id { get; set;  }
        public string Type { get; set; }
        public DateTime On { get; set; }
        public string EntityId { get; set; }
        public State Changes { get; set; }
    }
}
