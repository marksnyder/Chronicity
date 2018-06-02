using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core
{
    public interface IEventAgent
    {
        void OnEntityStateChange(string entity, string key, string priorValue, string newValue, DateTime on);
        void OnNewEvent(string entity, string type, DateTime on);
    }
}
