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
            var context = new Context() { Event = e, States = new List<EntityState>() };

            foreach (var entity in e.Entities)
            {
                if (_bank.TrackedState.Keys.Contains(entity))
                {
                    var match = _bank.TrackedState[entity].Where(x => x.On <= DateTime.Parse(e.On))
                      .OrderByDescending(x => x.On)
                      .FirstOrDefault();


                    if (match != null) context.States.Add(match.State);
                }
            }

            return context;
        }


  


        public void Track(Event e)
        {

            if (!_bank.EventTypes.Contains(e.Type)) _bank.EventTypes.Add(e.Type);

            var tslist = new List<TimeAndState>();

            //TODO - this wont hold up if observations are added out of sequence
            foreach(var entity in e.Entities)
            {
                if (_bank.TrackedState.Keys.Contains(entity))
                {
                    var ts = _bank.TrackedState[entity].Where(x => x.On >= DateTime.Parse(e.On)).OrderBy(x => x.On).FirstOrDefault();
                    if (ts != null)
                    {
                        tslist.Add(ts);
                    }
                }
            }

            _bank.StatefulEvents.Add(new StatefulEvent(e,tslist));


        }

        public void Track(Observation o)
        {
            TimeAndState tracked = null;

            tracked = new TimeAndState()
            {
                On = DateTime.Parse(o.On),
                State = new EntityState()
            };

            if (!_bank.TrackedState.ContainsKey(o.Entity))
            {
                _bank.TrackedState[o.Entity] = new List<TimeAndState>();
                _bank.TrackedState[o.Entity].Add(tracked);
            }
            else
            {
                TimeAndState prior;

                if (_bank.LastState.ContainsKey(o.Entity) && DateTime.Parse(o.On) > _bank.LastState[o.Entity])
                {
                    //optimize and get last - NOTE - this requires tracked state is in chronilogical order
                    prior = _bank.TrackedState[o.Entity].Last();
                }
                else
                {
                    prior = _bank.TrackedState[o.Entity].Where(x => x.On <= DateTime.Parse(o.On))
                    .OrderByDescending(x => x.On)
                    .FirstOrDefault();
                }

                if (prior != null)
                {
                    if (prior.On == DateTime.Parse(o.On))
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

                        _bank.TrackedState[o.Entity].Add(tracked);

                        if (tracked.On < prior.On)
                        {
                            _bank.TrackedState[o.Entity] = _bank.TrackedState[o.Entity].OrderBy(x => x.On).ToList(); // resync so we stay in order
                            // TODO - we need to remerge future states
                        }


                    }

                }
                else
                {
                    // Event is backdated and also   the oldest for this event
                    _bank.TrackedState[o.Entity].Add(tracked);
                    _bank.TrackedState[o.Entity] = _bank.TrackedState[o.Entity].OrderBy(x => x.On).ToList(); // resync so we stay in order

                    // TODO - we need to remerge future states
                }


            }

            if (o.Expressions != null)
            {
                var state = ParseStateChanges(o.Expressions);
                var link = ParseLinkChanges(o.Expressions);

                foreach (var change in state)
                {
                    tracked.State[change.Key] = change.Value;
                }

            }

            if (!_bank.LastState.ContainsKey(o.Entity) || DateTime.Parse(o.On) > _bank.LastState[o.Entity])
            {
                _bank.LastState[o.Entity] = DateTime.Parse(o.On);
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
