using Chronicity.Core.Events;
using System;
using System.Collections.Generic;

namespace Chronicity.Core
{
    public interface ITimelineService
    {
        void RegisterAgent(IStateChangeAgent agent);
        void ReprocessAgents(IEnumerable<IStateChangeAgent> agents, string key);

        IDictionary<string, string> GetEntityState(string entityid, string on);
        void RegisterEvent(NewEvent e);
        void RegisterObservation(Observation o);
        IEnumerable<Event> FilterEvents(IEnumerable<string> expressions);
        IList<string> SearchEventTypes(string search);
        IList<string> SearchEntities(string search);
        IList<Entity.StateRange> FilterState(IEnumerable<string> expressions);

    }
}
