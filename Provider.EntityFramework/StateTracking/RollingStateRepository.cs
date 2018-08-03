using Chronicity.Core.Reaction;
using Chronicity.Core.Timeline;
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
                if (lastChange == null || lastChange.Value != possibleChanges[key].StringValue)
                {
                    _context.TimeAndStates.Add(new TimeAndState()
                    {
                        Entity = o.Entity,
                        Key = key,
                        Value = possibleChanges[key].StringValue,
                        NumericValue = possibleChanges[key].NumericValue,
                        On = parsedTime,
                        PriorValue = lastChange != null ? lastChange.Value : null,
                        NumericPriorValue = lastChange != null ? lastChange.NumericValue : null
                        
                    });

                    _context.SaveChanges();

                    // Fire event agents
                    if(lastChange != null && lastChange.Value != possibleChanges[key].StringValue)
                    {

                        result.Add(new StateChange()
                        {
                            Entity = o.Entity,
                            Key = key,
                            OldValue = lastChange.Value,
                            NewValue = possibleChanges[key].StringValue,
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

                        nextChange.PriorValue = possibleChanges[key].StringValue;
                        nextChange.NumericPriorValue = possibleChanges[key].NumericValue;
                        _context.SaveChanges();

                        result.Add(new StateChange()
                        {
                            Entity = o.Entity,
                            Key = key,
                            OldValue = possibleChanges[key].StringValue,
                            NewValue = nextChange.Value,
                            On = nextChange.On.ToString("MM/dd/yyyy HH:mm:ss.fffffff")
                        });
                    }

                }
            }

            return result;
        }


        private Dictionary<string, ChangeValue> ParseStateObservations(IEnumerable<string> expressions)
        {
            var changes = new Dictionary<string, ChangeValue>();

            foreach (var observation in expressions)
            {
                if (observation.StartsWith("State."))
                {
                    var expression = observation.Replace("State.", "");
                    var var = expression.Split('=')[0];
                    var value = expression.Split('=')[1];

                    if (!changes.ContainsKey(var)) changes.Add(var, new ChangeValue());

                    changes[var].StringValue = value;

                    decimal numericVal;
                    if(Decimal.TryParse(value, out numericVal))
                    {
                        changes[var].NumericValue = numericVal;
                    }

                }
            }

            return changes;

        }

        public class ChangeValue
        {
            public string StringValue { get; set; }
            public decimal? NumericValue { get; set; }
        }
    }
}
