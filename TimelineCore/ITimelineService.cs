using Chronicity.Core.Events;
using System;
using System.Collections.Generic;

namespace Chronicity.Core
{
    public interface ITimelineService
    {

        IDictionary<string, string> GetEntityState(string entityid, string on);
        void RegisterEvent(Event e);
        void RegisterObservation(Observation o);
        IEnumerable<Context> FilterEvents(IEnumerable<string> expressions);
        IList<string> GetAllEventTypes();
       
    }
}
