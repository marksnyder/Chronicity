using Chronicity.Core.Events;
using Chronicity.Provider.EntityFramework.DataContext;
using Chronicity.Provider.EntityFramework.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Chronicity.Core;

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

            var keys = _context.EntityStateKeys.Where(x => x.Entity == entityid).Select(x => x.Key).ToList();

            var state = new EntityState();

            foreach(var key in keys)
            {
                var lastState = _context.TimeAndStates
                    .Where(x => x.Entity == entityid && x.Key == key && x.On <= parsedTime)
                    .OrderByDescending(x => x.On)
                    .FirstOrDefault();

                if(lastState != null) state.Add(key, lastState.Value);

            }

            return state;
        }

        public IList<StateChange> Track(Observation o)
        {
            var result = new List<StateChange>();

            DateTime parsedTime = DateTime.Parse(o.On);

            var possibleChanges = ParseStateObservations(o.Expressions);

            foreach(var key in possibleChanges.Keys)
            {
                // Update entity state keys if it doesn't exist in master list
                var keyMaster = _context.EntityStateKeys.FirstOrDefault(x => x.Key == key && x.Entity == o.Entity);

                if (keyMaster == null)
                {
                    keyMaster = new EntityStateKey()
                    {
                        Key = key,
                        Entity = o.Entity,
                        LastChange = parsedTime
                    };

                    _context.EntityStateKeys.Add(keyMaster);

                    _context.SaveChanges();
                }
                else if(keyMaster.LastChange == null || keyMaster.LastChange < parsedTime)
                {
                    // Update last change
                    keyMaster.LastChange = parsedTime;
                    _context.SaveChanges();
                }


                //Identify prior entity state 
                var lastChange = _context
                    .TimeAndStates
                    .Where(x => x.Key == key && x.Entity == o.Entity && x.On < parsedTime)
                    .OrderByDescending(x => x.On)
                    .FirstOrDefault();

                // Track
                if (lastChange == null || lastChange.Value != possibleChanges[key])
                {
                    _context.TimeAndStates.Add(new TimeAndState()
                    {
                        Entity = o.Entity,
                        Key = key,
                        Value = possibleChanges[key],
                        On = parsedTime,
                        PriorValue = lastChange != null ? lastChange.Value : null
                    });

                    _context.SaveChanges();

                    // Fire event agents
                    if(lastChange != null && lastChange.Value != possibleChanges[key])
                    {

                        result.Add(new StateChange()
                        {
                            Entity = o.Entity,
                            Key = key,
                            OldValue = lastChange.Value,
                            NewValue = possibleChanges[key],
                            On = o.On
                        });

                    }

                    // If this is a back-dated observation we need to fire new state change forward  & fix prior value of the next record ->
                    if(keyMaster.LastChange.Value > parsedTime)
                    {
                        var nextChange = _context
                           .TimeAndStates
                           .Where(x => x.Key == key && x.Entity == o.Entity && x.On > parsedTime)
                           .OrderBy(x => x.On)
                           .FirstOrDefault();

                        nextChange.PriorValue = possibleChanges[key];
                        _context.SaveChanges();

                        result.Add(new StateChange()
                        {
                            Entity = o.Entity,
                            Key = key,
                            OldValue = possibleChanges[key],
                            NewValue = nextChange.Value,
                            On = nextChange.On.ToString("MM/dd/yyyy HH:mm:ss.fffffff")
                        });
                    }

                }
            }

            return result;
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
