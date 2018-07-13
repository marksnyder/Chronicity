using Chronicity.Core.Timeline;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core.Reaction
{
    public class ReactionResult
    {
        public IList<NewEvent> NewEvents { get; set; }
        public IList<Observation> NewObservations { get; set; }
    }
}
