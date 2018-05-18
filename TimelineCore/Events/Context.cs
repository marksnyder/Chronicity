using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core.Events
{
    public class Context
    {
        public Event Event { get; set; }
        public IList<EntityState> States { get; set; }
    }
}
