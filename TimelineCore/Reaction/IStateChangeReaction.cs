using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core.Reaction
{
    public interface IStateChangeReaction
    {
        ReactionResult OnChange(string entity, string key, string priorValue, string newValue, string on);
    }
}
