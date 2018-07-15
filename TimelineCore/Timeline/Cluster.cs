﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core.Timeline
{
    public class Cluster
    {
        public string Start { get; set; }
        public string End { get; set; }
        public IList<ExistingEvent> Events { get; set; }
        public IEnumerable<string> Entities { get; set; }
    }
}
