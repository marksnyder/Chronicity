﻿using Chronicity.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core.Agent
{
    public class StateChangeResult
    {
        public IList<NewEvent> NewEvents { get; set; }
        public IList<Observation> NewObservations { get; set; }
    }
}
