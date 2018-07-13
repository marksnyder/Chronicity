using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core.Timeline
{
    public class Context
    {
        public ExistingEvent Event { get; set; }
        public IList<EntityState> States { get; set; }
    }
}
