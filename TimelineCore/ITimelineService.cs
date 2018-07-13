using Chronicity.Core.Timeline;
using Chronicity.Core.Reaction;
using System;
using System.Collections.Generic;

namespace Chronicity.Core
{
    public interface ITimelineService
    {
        void RegisterReaction(IStateChangeReaction reaction);
        void RunReaction(IStateChangeReaction reaction);

        IDictionary<string, string> GetEntityState(string entityid, string on);
        void RegisterEvent(NewEvent e);
        void RegisterObservation(Observation o);
        IEnumerable<ExistingEvent> FilterEvents(IEnumerable<string> expressions);
        IList<string> SearchEventTypes(string search);
        IList<string> SearchEntities(string search);
        IList<StateRange> FilterState(IEnumerable<string> expressions);
        IList<Cluster> SearchClusters(IEnumerable<string> filterExpressions, IEnumerable<string> clusterExpressions);

    }
}
