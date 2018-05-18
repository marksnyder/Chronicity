using Chronicity.Core;
using Chronicity.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Chronicity.Provider.InMemory.StateTracking;
using Chronicity.Provider.InMemory.Data;

namespace Chronicity.Provider.InMemory
{
    public class TimeLineService : ITimelineService
    {
        private DataBank _Bank;
        private RollingStateRepository _StateRepository;

        public TimeLineService()
        {
            _Bank  = new DataBank();
            _StateRepository = new RollingStateRepository(_Bank);
        }

        public IDictionary<string, string> GetEntityState(string entityid, string on)
        {
            return _StateRepository.GetEntityState(entityid, on);
        }


        public void RegisterEvent(Event e)
        {
            if (e.Entities == null) throw new Exception("You must specify entities");

            _StateRepository.Track(e);

        }

        public void RegisterObservation(Observation o)
        {
            _StateRepository.Track(o);
        }

        public IEnumerable<Context> FilterEvents(IEnumerable<string> expressions)
        {
            var events = _Bank.Events.AsEnumerable();

            foreach(var expression in expressions)
            {
                events = ParseFilterEventExpressions(expression, events);
            }


            //TODO -- need to optimize this..
            var contexts = events.ToList().Select(x => new Context()
            {
                 Event = x,
                 States = x.Entities.Select(y => _StateRepository.GetEntityState(y,x.On)).ToList()
            }); 

            foreach(var expression in expressions)
            {
                contexts = ParseFilterContextExpression(expression, contexts);
            }

            return contexts;

        }


        private IEnumerable<Event> ParseFilterEventExpressions(string expression, IEnumerable<Event> events)
        {
            var ret = events;

            if (expression.StartsWith("Type"))
            {
                var value = expression.Split('=')[1];
                ret = ret.Where(x => x.Type == value);
            }
            else if (expression.StartsWith("On."))
            {
                var e = expression.Replace("On.", "");
                var action = e.Split('=')[0];

                if (action == "After")
                {
                    var value = DateTime.Parse(e.Split('=')[1]);
                    ret = ret.Where(x => DateTime.Parse(x.On) > value);
                }
                else if (action == "Before")
                {
                    var value = DateTime.Parse(e.Split('=')[1]);
                    ret = ret.Where(x => DateTime.Parse(x.On) < value);
                }
                else if (action == "Between")
                {
                    var value = e.Split('=')[1].Split(',');
                    var value1 = DateTime.Parse(value[0]);
                    var value2 = DateTime.Parse(value[1]);
                    ret = ret.Where(x => DateTime.Parse(x.On) > value1 && DateTime.Parse(x.On) < value2);
                }
            }

            return ret.AsEnumerable();
        }

        private IEnumerable<Context> ParseFilterContextExpression(string expression, IEnumerable<Context> contexts)
        {
            var ret = contexts;

            if (expression.StartsWith("Entity.State."))
            {
                var e = expression.Replace("Entity.State.", "");
                var var = e.Split('=')[0];
                var value = e.Split('=')[1];
                ret = ret.Where(x => x.States.Count(y=> y.ContainsKey(var) && y[var] == value) > 0);
            }
           
            return ret;
        }

        public IList<string> GetAllEventTypes()
        {
            return _Bank.EventTypes;
        }

    }
}
