using System;
using System.Collections.Generic;
using System.Text;

namespace Provider.EntityFramework.StateTracking
{
    public class StateChange
    {
        public string Key { get; set; }
        public string Entity { get; set; }
        public string NewValue { get; set; }
        public string OldValue { get; set; }
        public string On { get; set; }
    }
}
