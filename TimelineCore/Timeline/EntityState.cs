using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core.Timeline
{
    public class EntityState : Dictionary<string,string>
    {
        public string Entity { get; set; }
    }
}
