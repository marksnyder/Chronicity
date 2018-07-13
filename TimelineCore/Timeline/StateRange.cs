using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core.Timeline
{
    public class StateRange
    {
        public string Entity { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
