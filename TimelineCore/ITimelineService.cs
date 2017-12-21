using Chronicity.Core.Events;
using System;
using System.Collections.Generic;

namespace Chronicity.Core
{
    public interface ITimelineService
    {
        void RegisterEntity(string id, string type);

        string GetEntityType(string id);
        IDictionary<string, string> GetEntityState(string entityid, string on);

        void RegisterEvent(Event e);
        IEnumerable<Context> FilterEvents(IEnumerable<string> expressions);
        
   
    }
}
