using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core.Timeline
{
    public class NewEvent
    {
        public string Type { get; set; }
        public string On { get; set; }
        public IList<string> Entities { get; set; }
    }
}
