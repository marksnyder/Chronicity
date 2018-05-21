using Chronicity.Core;
using Chronicity.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Chronicity.Provider.EntityFramework.DataContext;
using Provider.EntityFramework.StateTracking;

namespace Chronicity.Provider.EntityFramework
{
    public class TimeLineService : ITimelineService
    {
        private ChronicityContext _context;
        private RollingStateRepository _stateRepository;

        public TimeLineService(ChronicityContext context)
        {
            _context = context;
            _stateRepository = new RollingStateRepository(context);
        }

        public IDictionary<string, string> GetEntityState(string entityid, string on)
        {
            return _stateRepository.GetEntityState(entityid, on);
        }


        public void RegisterEvent(Event e)
        {
            if (e.Entities == null) throw new Exception("You must specify entities");

            _context.Events.Add(new Provider.EntityFramework.DataModels.Event()
            {
                 EntityList = String.Join(',',e.Entities),
                 On = DateTime.Parse(e.On),
                 Type = e.Type
            });

            if(_context.EventTypes.Count(x => x.Type == e.Type) == 0)
            {
                _context.EventTypes.Add(new DataModels.EventType()
                {
                    Type = e.Type
                });
            }

            _context.SaveChanges();
        }

        public void RegisterObservation(Observation o)
        {
            _stateRepository.Track(o);
        }

        public IEnumerable<Context> FilterEvents(IEnumerable<string> expressions)
        {
            var events = _context.Events.AsQueryable();

            foreach (var expression in expressions)
            {
                events = ParseFilterEventExpressions(expression, events);
            }

            //TODO -- need to optimize this.. we shouldn't have to pull all of them back to do the filtering
            var contexts = events.ToList().Select(x => new Context()
            { 
                Event = new Event() { Entities = x.EntityList.Split(",") , On = x.On.ToString("yyyy/MM/dd HH:mm:ss"), Type = x.Type },
                States = x.EntityList.Split(",").Select(y => _stateRepository.GetEntityState(y, x.On.ToString("MM/dd/yyyy HH:mm:ss"))).ToList()
            });

            foreach (var expression in expressions)
            {
                contexts = ParseFilterContextExpression(expression, contexts);
            }

            return contexts;
        }


        private IQueryable<DataModels.Event> ParseFilterEventExpressions(string expression, IQueryable<DataModels.Event> events)
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
                    ret = ret.Where(x => x.On > value);
                }
                else if (action == "Before")
                {
                    var value = DateTime.Parse(e.Split('=')[1]);
                    ret = ret.Where(x =>x.On < value);
                }
                else if (action == "Between")
                {
                    var value = e.Split('=')[1].Split(',');
                    var value1 = DateTime.Parse(value[0]);
                    var value2 = DateTime.Parse(value[1]);
                    ret = ret.Where(x => x.On > value1 && x.On < value2);
                }
            }

            return ret.AsQueryable();
        }

        private IEnumerable<Context> ParseFilterContextExpression(string expression, IEnumerable<Context> contexts)
        {
            var ret = contexts;

            if (expression.StartsWith("Entity.State."))
            {
                var e = expression.Replace("Entity.State.", "");
                var var = e.Split('=')[0];
                var value = e.Split('=')[1];
                ret = ret.Where(x => x.States.Count(y => y.ContainsKey(var) && y[var] == value) > 0);
            }

            return ret;
        }

        public IList<string> GetAllEventTypes()
        {
            return _context.EventTypes.Select(x => x.Type).ToList();
        }

    }
}
