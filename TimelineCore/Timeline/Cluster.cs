using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core.Timeline
{
    public class Cluster
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public IList<ExistingEvent> Events { get; set; }
    }
}
