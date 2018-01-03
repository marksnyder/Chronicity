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
        private List<Event> Events = new List<Event>();
        private Dictionary<string,List<TimeTrackedState>> TrackedState = new Dictionary<string, List<TimeTrackedState>>();

        public IDictionary<string, string> GetEntityState(string entityid, string on)
        {
            if (!TrackedState.ContainsKey(entityid)) return new Dictionary<string, string>();

            var match = TrackedState[entityid].Where( x=> x.On <= DateTime.Parse(on))
                .OrderByDescending(x => x.On)
                .FirstOrDefault();

            var ret = new Dictionary<string, string>();

            foreach(var key in match.State.Keys)
            {
                ret[key] = match.State[key];
            }

            foreach(var link in match.Links)
            {
                var linkSate = GetEntityState(link, on);

                foreach (var key in match.State.Keys)
                {
                    ret[string.Concat(link,".",key)]= match.State[key];
                }
            }

            return match.State;
        }

        public void RegisterEvent(Event e)
        {
            if (String.IsNullOrEmpty(e.Entity)) throw new Exception("You must specify an entity id");

            Events.Add(e);
            TimeTrackedState tracked = null;

            tracked = new TimeTrackedState()
            {
                On = DateTime.Parse(e.On),
                State = new Dictionary<string, string>(),
                Links = new List<string>()
            };

            if (!TrackedState.ContainsKey(e.Entity))
            {
                TrackedState[e.Entity] = new List<TimeTrackedState>();
                TrackedState[e.Entity].Add(tracked);
            }
            else
            {
                var prior = TrackedState[e.Entity].Where(x => x.On <= DateTime.Parse(e.On))
                .OrderByDescending(x => x.On)
                .FirstOrDefault();

                if (prior != null)
                {
                    if(prior.On == DateTime.Parse(e.On))
                    {
                        // Event is concurrent with another event
                        tracked = prior;
                    }
                    else
                    {
                        // Event is either in the middle or after
                        foreach (var key in prior.State.Keys)
                        {
                            tracked.State[key] = prior.State[key];
                        }

                        TrackedState[e.Entity].Add(tracked);
                    }

                }
                else
                {
                    // Event is backdated and now the oldest for this event
                    TrackedState[e.Entity].Add(tracked);
                }


            }

           

            if (e.Observations != null)
            {
                foreach (var observation in e.Observations)
                {
                    ParseObservation(observation, tracked.State, tracked.Links);
                }
            }



        }

        public IEnumerable<Context> FilterEvents(IEnumerable<string> expressions)
        {

            var contexts = Events.Select(x => new Context()
            {
                Event = x,
                State = TrackedState[x.Entity].Where(xx =>  DateTime.Parse(x.On) >= xx.On).OrderByDescending(xx => xx.On).First().State,
                Links = TrackedState[x.Entity].Where(xx =>  DateTime.Parse(x.On) >= xx.On).OrderByDescending(xx => xx.On).First().Links,
                LinkedState = GetLinkedState(TrackedState[x.Entity].Where(xx => DateTime.Parse(x.On) >= xx.On).OrderByDescending(xx => xx.On).First().Links, DateTime.Parse(x.On))

            }); 

            foreach(var expression in expressions)
            {
                contexts = ParseFilterExpression(expression, contexts);
            }

            return contexts;

        }

        private Dictionary<string,Dictionary<string,string>> GetLinkedState(List<string> links, DateTime on)
        {
            var ret = new Dictionary<string, Dictionary<string, string>>();

            foreach(var link in links)
            {
                if (TrackedState.ContainsKey(link))
                {
                    var match = TrackedState[link].Where(x => x.On <= on)
                   .OrderByDescending(x => x.On)
                   .FirstOrDefault();

                    if (match != null) ret[link] = match.State;
                }
            }

            return ret;
        }


        private void ParseObservation(string observation, Dictionary<string,string> state, List<string> links)
        {
            if(observation.StartsWith("Entity.State."))
            {
                var expression = observation.Replace("Entity.State.", "");
                var var = expression.Split('=')[0];
                var value = expression.Split('=')[1];
                state[var] = value;
            }
            else if (observation.StartsWith("Entity.Links.Add"))
            {
                var expression = observation.Replace("Entity.Links.Add", "");
                var var = expression.Split('=')[0];
                var value = expression.Split('=')[1];
                links.Add(value);
            }
        }

        private IEnumerable<Context> ParseFilterExpression(string expression, IEnumerable<Context> contexts)
        {
            var ret = contexts;

            if (expression.StartsWith("Entity.State."))
            {
                var e = expression.Replace("Entity.State.", "");
                var var = e.Split('=')[0];
                var value = e.Split('=')[1];
                ret = ret.Where(x => x.State.ContainsKey(var) && x.State[var] == value);
            }
            else if (expression.StartsWith("Type"))
            {
                var value = expression.Split('=')[1];
                ret = ret.Where(x => x.Event.Type == value);
            }
            else if (expression.StartsWith("On."))
            {
                var e = expression.Replace("On.", "");
                var action = e.Split('=')[0];

                if (action == "After")
                {
                    var value = DateTime.Parse(e.Split('=')[1]);
                    ret = ret.Where(x => DateTime.Parse(x.Event.On) > value);
                }
                else if (action == "Before")
                {
                    var value = DateTime.Parse(e.Split('=')[1]);
                    ret = ret.Where(x => DateTime.Parse(x.Event.On) < value);
                }
                else if (action == "Between")
                {
                    var value = e.Split('=')[1].Split(',');
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
            public List<string> Links { get; set; }
            public DateTime On { get; set; }
        }
        
    }
}
