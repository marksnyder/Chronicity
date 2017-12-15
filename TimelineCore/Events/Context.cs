using Chronicity.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core.Events
{
    public class Context
    {
        public Event Event { get; set; }
        public State State { get; set; } 
    }
}
