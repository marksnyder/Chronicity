using Chronicity.Core.Events;
using Chronicity.Provider.EntityFramework.DataContext;
using Chronicity.Provider.EntityFramework.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Provider.EntityFramework.StateTracking
{
    public class RollingStateRepository
    {
        private ChronicityContext _context;

        public RollingStateRepository(ChronicityContext context)
        {
            _context = context;
        }

        public EntityState GetEntityState(string entityid, string on)
        {
            DateTime parsedTime = DateTime.Parse(on);
            var tracked =_context.TimeAndStates
                .Where(x => x.Entity == entityid && x.On <= parsedTime).OrderByDescending(x => x.On).FirstOrDefault();

            var state = new EntityState() { Entity = entityid };

            if(tracked != null)
            {
                var entries = _context.TimeAndStateEntries.Where(x => x.TimeAndStateEntryId == tracked.Id);

                foreach(var e in entries)
                {
                    state.Add(e.Key, e.Value);
                }
            }

            return state;
        }

        public void Track(Observation o)
        {
            DateTime parsedTime = DateTime.Parse(o.On);
            TimeAndState tracked = new TimeAndState() { On = DateTime.Parse(o.On), Entity = o.Entity };
            List<TimeAndStateEntry> trackedEntries = new List<TimeAndStateEntry>();


            TimeAndState prior;

            prior = _context.TimeAndStates.Where(x => x.Entity == o.Entity && x.On <= parsedTime ).OrderByDescending(x => x.On).FirstOrDefault();

            if (prior != null)
            {
                if (prior.On == parsedTime)
                {
                    // Event is concurrent with another event
                    tracked = prior;
                    trackedEntries = _context.TimeAndStateEntries.Where(x => x.TimeAndStateEntryId == prior.Id).ToList();
                }
                else
                {
                    // Event is either in the middle or after
                    var priorEntries = _context.TimeAndStateEntries.Where(x => x.TimeAndStateEntryId == prior.Id);

                    _context.TimeAndStates.Add(tracked);
                    _context.SaveChanges();

                    foreach (var e in priorEntries)
                    {
                        var trackE = new TimeAndStateEntry()
                        {
                            Key = e.Key,
                            Value = e.Value,
                            TimeAndStateEntryId = tracked.Id
                        };

                        trackedEntries.Add(trackE);
                        _context.TimeAndStateEntries.Add(trackE);
                        _context.SaveChanges();
                    }

                }

            }
            else
            {
                _context.TimeAndStates.Add(tracked);
                _context.SaveChanges();
            }



            if (o.Expressions != null)
            {
                var state = ParseStateChanges(o.Expressions);

                foreach (var change in state)
                {
                    var trackE = trackedEntries.Where(x => x.Key == change.Key).FirstOrDefault();

                    if(trackE == null)
                    {
                        trackE = new TimeAndStateEntry()
                        {
                            Key = change.Key,
                            TimeAndStateEntryId = tracked.Id
                        };

                        trackedEntries.Add(trackE);
                        _context.TimeAndStateEntries.Add(trackE);
                    }

                    trackE.Value = change.Value;
                }

                _context.SaveChanges();

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
    }
}
