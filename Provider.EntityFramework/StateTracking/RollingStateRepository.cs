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

            var prior = _context
                .TimeAndStates
                .Where(x => x.Entity == o.Entity && x.On <= parsedTime )
                .OrderByDescending(x => x.On)
                .FirstOrDefault();

            if (prior != null)
            {
                var priorEntries = _context.TimeAndStateEntries.Where(x => x.TimeAndStateEntryId == prior.Id);
                var newEntries = GenerateEntries(o.Expressions, priorEntries);

                if (prior.On == parsedTime)
                {
                    // Observation is concurrent with another change in state
                    foreach(var e in newEntries)
                    {
                        //Only merge in new keys
                        if (!priorEntries.Select(x => x.Key).Contains(e.Key))
                        {
                            e.TimeAndStateEntryId = prior.Id;
                            _context.TimeAndStateEntries.Add(e);
                        }
                    }
                    _context.SaveChanges();
                }
                else
                {
                    // Observation is either in the middle or after
                    if (newEntries.Count() > 0)
                    {
                        var newEntry = new TimeAndState() { Entity = o.Entity, On = parsedTime };
                        _context.TimeAndStates.Add(newEntry);
                        _context.SaveChanges();

                        foreach (var e in newEntries)
                        {
                            e.TimeAndStateEntryId = newEntry.Id;
                            _context.TimeAndStateEntries.Add(e);
                        }

                        // Each snapshot in state (at least right now) needs to be a full one. 
                        // So given that we have change in state we need snapshot others as well

                        foreach(var e in priorEntries)
                        {
                            if (!newEntries.Select(x => x.Key).Contains(e.Key))
                            {
                                _context.TimeAndStateEntries.Add(new TimeAndStateEntry()
                                {
                                    Key = e.Key,
                                    Value = e.Value,
                                    TimeAndStateEntryId = newEntry.Id
                                });
                            }
                            _context.SaveChanges();
                        }

                        _context.SaveChanges();
                    }
                }

            }
            else
            {
                // There is no prior entry - possibly the first or this was backdated
                var items = GenerateEntries(o.Expressions, new List<TimeAndStateEntry>());

                if (items.Count() > 0)
                {
                    var newEntry = new TimeAndState() { Entity = o.Entity, On = parsedTime };
                    _context.TimeAndStates.Add(newEntry);
                    _context.SaveChanges();

                    foreach (var e in items)
                    {
                        e.TimeAndStateEntryId = newEntry.Id;
                        _context.TimeAndStateEntries.Add(e);
                    }
                    _context.SaveChanges();
                }
            }

        }

        private IList<TimeAndStateEntry> GenerateEntries(IEnumerable<string> expressions, IEnumerable<TimeAndStateEntry> priorEntries)
        {
            var newEntries = new List<TimeAndStateEntry>();

            var observations = ParseStateObservations(expressions);
            bool changed = false;

            foreach (var key in observations.Keys)
            {
                var prior = priorEntries.Where(x => x.Key == key).FirstOrDefault();
                if (prior == null || prior.Value != observations[key])
                {
                    changed = true;
                    newEntries.Add(new TimeAndStateEntry()
                    {
                        Key = key,
                        Value = observations[key]
                    });
                }
                else
                {
                    newEntries.Add(new TimeAndStateEntry()
                    {
                        Key = key,
                        Value = prior.Value
                    });
                }

            }

            if (!changed) return new List<TimeAndStateEntry>();

            return newEntries;
        }

        private Dictionary<string, string> ParseStateObservations(IEnumerable<string> expressions)
        {
            var changes = new Dictionary<string, string>();

            foreach (var observation in expressions)
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
