using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Chronicity.Core.Events;
using Chronicity.Provider.InMemory.Data;

namespace Chronicity.Provider.InMemory.StateTracking
{
    public class RollingStateRepository
    {
        private DataBank _bank;

        public RollingStateRepository(DataBank bank)
        {
            _bank = bank;
        }

        public EntityState GetEntityState(string entityid, string on)
        {
            if (!_bank.TrackedState.ContainsKey(entityid)) return new EntityState();

            var match = _bank.TrackedState[entityid].Where(x => x.On <= DateTime.Parse(on))
                .OrderByDescending(x => x.On)
                .FirstOrDefault();

            return match.State;
        }

        public Context GetEventContext(Event e)
        {
            if (!_bank.TrackedState.ContainsKey(e.Entity)) return new Context()
            {
                Event = e,
                Links = new List<string>(),
                State = new EntityState()
            };

            var match = _bank.TrackedState[e.Entity].Where(x => x.On <= DateTime.Parse(e.On))
              .OrderByDescending(x => x.On)
              .FirstOrDefault();

            var result = new Context()
            {

                Event = e,
                State = match.State,
                Links = match.Links
            };

            return result;
        }


  
        public List<string> GetEntityLinks(string entityid, string on)
        {
            if (!_bank.TrackedState.ContainsKey(entityid)) return new List<string>();

            var match = _bank.TrackedState[entityid].Where(x => x.On <= DateTime.Parse(on))
                .OrderByDescending(x => x.On)
                .FirstOrDefault();

            return match.Links;
        }

        public void Track(Event e)
        {

            TimeAndState tracked = null;

            tracked = new TimeAndState()
            {
                On = DateTime.Parse(e.On),
                State = new EntityState(),
                Links = new List<string>()
            };

            if (!_bank.TrackedState.ContainsKey(e.Entity))
            {
                _bank.TrackedState[e.Entity] = new List<TimeAndState>();
                _bank.TrackedState[e.Entity].Add(tracked);
            }
            else
            {
                TimeAndState prior;

                if (_bank.LastState.ContainsKey(e.Entity) && DateTime.Parse(e.On) > _bank.LastState[e.Entity])
                {
                    //optimize and get last - NOTE - this requires tracked state is in chronilogical order
                    prior = _bank.TrackedState[e.Entity].Last();
                }
                else
                {
                    prior = _bank.TrackedState[e.Entity].Where(x => x.On <= DateTime.Parse(e.On))
                    .OrderByDescending(x => x.On)
                    .FirstOrDefault();
                }

                if (prior != null)
                {
                    if (prior.On == DateTime.Parse(e.On))
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

                        _bank.TrackedState[e.Entity].Add(tracked);

                        if (tracked.On < prior.On)
                        {
                            _bank.TrackedState[e.Entity] = _bank.TrackedState[e.Entity].OrderBy(x => x.On).ToList(); // resync so we stay in order
                            // TODO - we need to remerge future states
                        }


                    }

                }
                else
                {
                    // Event is backdated and also   the oldest for this event
                    _bank.TrackedState[e.Entity].Add(tracked);
                    _bank.TrackedState[e.Entity] = _bank.TrackedState[e.Entity].OrderBy(x => x.On).ToList(); // resync so we stay in order

                    // TODO - we need to remerge future states
                }


            }

            if (e.Observations != null)
            {
                var state = ParseStateChanges(e.Observations);
                var link = ParseLinkChanges(e.Observations);

                foreach (var change in state)
                {
                    tracked.State[change.Key] = change.Value;
                }

                foreach (var change in link)
                {
                    if (!tracked.Links.Contains(change)) tracked.Links.Add(change);
                }
            }

            _bank.StatefulEvents.Add(new StatefulEvent(e,tracked));


            if (!_bank.LastState.ContainsKey(e.Entity) || DateTime.Parse(e.On) > _bank.LastState[e.Entity])
            {
                _bank.LastState[e.Entity] = DateTime.Parse(e.On);
            }
        }

        private Dictionary<string, string> ParseStateChanges(IEnumerable<string> observations)
        {
            var changes = new Dictionary<string, string>();

            foreach (var observation in observations)
            {
                if (observation.StartsWith("Entity.State."))
                {
                    var expression = observation.Replace("Entity.State.", "");
                    var var = expression.Split('=')[0];
                    var value = expression.Split('=')[1];
                    changes[var] = value;
                }
            }

            return changes;

        }

        private List<string> ParseLinkChanges(IEnumerable<string> observations)
        {
            var changes = new List<string>();

            foreach (var observation in observations)
            {
                if (observation.StartsWith("Entity.Links.Add"))
                {
                    var expression = observation.Replace("Entity.Links.Add", "");
                    var var = expression.Split('=')[0];
                    var value = expression.Split('=')[1];
                    changes.Add(value);
                }
            }

            return changes;
        }
    }
}
