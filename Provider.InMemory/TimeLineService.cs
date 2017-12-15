using Chronicity.Core;
using Chronicity.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Chronicity.Provider.InMemory
{
    public class TimeLineService : ITimelineService
    {
        private Dictionary<string, string> EntityRegistrations = new Dictionary<string, string>();
        private List<Event> Events = new List<Event>();

        public void RegisterEvent(Event e)
        {
            if (String.IsNullOrEmpty(e.EntityId)) throw new Exception("You must specify an entity id");
            if (!EntityRegistrations.Keys.Contains(e.EntityId)) throw new Exception("Unable to add event for unregistered entity type");

            Events.Add(e);
        }

        public IEnumerable<Context> FilterEvents(Filter filter)
        {
            return Events.Select(x => new Context() {  Event = x });
        }

        public string GetEntityType(string id)
        {
            return EntityRegistrations[id];
        }

        public void RegisterEntity(string id, string type)
        {
            EntityRegistrations.Add(id, type);
        }
    }
}
