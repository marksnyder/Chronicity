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
        private List<TimeTrackedState> TrackedState = new List<TimeTrackedState>();

        public void RegisterEvent(Event e)
        {
            if (String.IsNullOrEmpty(e.Entity)) throw new Exception("You must specify an entity id");
            if (!EntityRegistrations.Keys.Contains(e.Entity)) throw new Exception("Unable to add event for unregistered entity type");

            Events.Add(e);
            var tracked = new TimeTrackedState()
            {
                Entity = e.Entity,
                On = DateTime.Parse(e.On),
                State = new Dictionary<string, string>()
            };

            // Merge prior state

            var prior = TrackedState.Where(x => x.Entity == tracked.Entity && x.On < tracked.On).OrderByDescending(x => x.On).FirstOrDefault();

            if (prior != null)
            {
                foreach(var key in prior.State.Keys)
                {
                    tracked.State[key] = prior.State[key];
                }
            }

            // Parse expressions

            if (e.Observations != null)
            {
                foreach (var observation in e.Observations)
                {
                    ParseObservation(observation, tracked.State);
                }
            }

            TrackedState.Add(tracked);
            
        }

        public IEnumerable<Context> FilterEvents(IEnumerable<string> expressions)
        {

            var contexts = Events.Select(x => new Context()
            {
                Event = x,
                State = TrackedState
                .Where(xx => x.Entity == xx.Entity && DateTime.Parse(x.On) >= xx.On)
                .OrderByDescending(xx => xx.On)
                .First()
                .State
            }); 

            foreach(var expression in expressions)
            {
                contexts = ParseFilterExpression(expression, contexts);
            }

            return contexts;

        }

        public string GetEntityType(string id)
        {
            return EntityRegistrations[id];
        }

        public void RegisterEntity(string id, string type)
        {
            EntityRegistrations.Add(id, type);
        }

        private void ParseObservation(string observation, Dictionary<string,string> state)
        {
            if(observation.StartsWith("State."))
            {
                var expression = observation.Replace("State.", "");
                var var = expression.Split("=")[0];
                var value = expression.Split("=")[1];
                state[var] = value;
            }
        }

        private IEnumerable<Context> ParseFilterExpression(string expression, IEnumerable<Context> contexts)
        {
            var ret = contexts;

            if (expression.StartsWith("State."))
            {
                var e = expression.Replace("State.", "");
                var var = e.Split("=")[0];
                var value = e.Split("=")[1];
                ret = ret.Where(x => x.State.ContainsKey(var) && x.State[var] == value);
            }
            else if(expression.StartsWith("On."))
            {
                var e = expression.Replace("On.", "");
                var action = e.Split("=")[0];            

                if(action == "After")
                {
                    var value = DateTime.Parse(e.Split("=")[1]);
                    ret = ret.Where(x => DateTime.Parse(x.Event.On) > value);
                }
                else if (action == "Before")
                {
                    var value = DateTime.Parse(e.Split("=")[1]);
                    ret = ret.Where(x => DateTime.Parse(x.Event.On) < value);
                }
                else if (action == "Between")
                {
                    var value = e.Split("=")[1].Split(",");
                    var value1 = DateTime.Parse(value[0]);
                    var value2 = DateTime.Parse(value[1]);
                    ret = ret.Where(x => DateTime.Parse(x.Event.On) > value1 && DateTime.Parse(x.Event.On) < value2);
                }        
            }

            return ret;
        }

        public class TimeTrackedState
        {
            public Dictionary<string,string> State { get; set; }
            public DateTime On { get; set; }
            public string Entity { get; set; }
        }
        
    }
}
