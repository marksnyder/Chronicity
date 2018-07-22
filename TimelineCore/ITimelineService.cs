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
        IEnumerable<ExistingEvent> SearchEvents(IEnumerable<string> expressions);
        IList<string> SearchEventTypes(string search);
        IList<string> SearchEntities(string search);
        IList<StateRange> SearchState(IEnumerable<string> expressions);
        IList<Cluster> ClusterEvents(IEnumerable<string> filterExpressions, IEnumerable<string> clusterExpressions);

    }
}
