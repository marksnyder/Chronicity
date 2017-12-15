using System;
using System.Collections.Generic;
using System.Text;
using Chronicity.Core.Entities;

namespace Chronicity.Core.Events
{
    public class Filter
    {
        public IEnumerable<string> EventType { get; set; }
        public IEnumerable<string> EntityId { get; set; }
        public IEnumerable<string> EntityType { get; set; }

    }
}
