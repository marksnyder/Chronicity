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

        public void Track(Observation o)
        {
            DateTime parsedTime = DateTime.Parse(o.On);

            var possibleChanges = ParseStateObservations(o.Expressions);

            foreach(var key in possibleChanges.Keys)
            {
                if(_context.EntityStateKeys.FirstOrDefault(x => x.Key == key) == null)
                {
                    _context.EntityStateKeys.Add(new EntityStateKey()
                    {
                        Key = key,
                        Entity = o.Entity
                    });

                    _context.SaveChanges();
                }

                var lastChange = _context
                    .TimeAndStates
                    .Where(x => x.Key == key && x.Entity == o.Entity && x.On < parsedTime)
                    .OrderByDescending(x => x.On)
                    .FirstOrDefault();

                if (lastChange == null || lastChange.Value != possibleChanges[key])
                {
                    _context.TimeAndStates.Add(new TimeAndState()
                    {
                         Entity = o.Entity,
                         Key = key,
                         Value = possibleChanges[key],
                         On = parsedTime
                    });

                    _context.SaveChanges();
                }
            }
          

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
