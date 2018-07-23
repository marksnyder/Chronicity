using Chronicity.Core;
using Chronicity.Core.Reaction;
using Chronicity.Core.Timeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleCommon
{
    public class FeederRefill : IStateChangeReaction
    {
        public ReactionResult OnChange(string entity, string key, string priorValue, string newValue, string on)
        {
            var result = new ReactionResult();

            if (key == "proxa" && priorValue != string.Empty)
            {
                if (Convert.ToInt32(newValue) < 9 && Convert.ToInt32(priorValue) >= 11)
                {
                    result.NewEvents = new List<NewEvent>() { new NewEvent() {
                         Entities = new string [] { entity },
                         On = on,
                         Type = "Feeder Refill"
                    } };
                }
            }

            return result;
        }
    }
}
