using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core.Timeline
{
    public class Observation
    {
        public string Type { get; set; }
        public string On { get; set; }
        public string Entity { get; set; }
        public IList<string> Expressions { get; set; }
    }
}
