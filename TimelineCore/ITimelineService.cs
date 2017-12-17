using Chronicity.Core.Events;
using System;
using System.Collections.Generic;

namespace Chronicity.Core
{
    public interface ITimelineService
    {
        void RegisterEntity(string id, string type);
        string GetEntityType(string id);
        void RegisterEvent(Event e);
        IEnumerable<Context> FilterEvents(IEnumerable<string> exrpessions);
   
    }
}
