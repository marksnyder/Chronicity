using Chronicity.Core;
using Chronicity.Core.Reaction;
using Chronicity.Core.Timeline;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Chronicity.Provider.EntityFramework.DataContext;
using Provider.EntityFramework.StateTracking;
using Chronicity.Provider.EntityFramework.DataModels;

namespace Chronicity.Provider.EntityFramework
{
    public class TimeLineService : ITimelineService
    {
        private ChronicityContext _context;
        private RollingStateRepository _stateRepository;
        private IList<IStateChangeReaction> _eventAgents;


        public TimeLineService(ChronicityContext context)
        {
            _context = context;
            _eventAgents = new List<IStateChangeReaction>();
            _stateRepository = new RollingStateRepository(context);

        }

        public void RegisterReaction(IStateChangeReaction reaction)
        {
            _eventAgents.Add(reaction);
        }

        public void RunReaction(IStateChangeReaction reaction)
        {
            var changes = _context.TimeAndStates.ToList();

            foreach (var c in changes)
            {
                var result = reaction.OnChange(c.Entity, c.Key, c.PriorValue, c.Value, c.On.ToString("MM/dd/yyyy HH:mm:ss.fffffff"));

                if (result.NewObservations != null)
                {
                    foreach (var ob in result.NewObservations) { this.RegisterObservation(ob); }
                }
                if (result.NewEvents != null)
                {
                    foreach (var ev in result.NewEvents) { this.RegisterEvent(ev); }
                }
            }
        }

        public IDictionary<string, string> GetEntityState(string entityid, string on)
        {
            return _stateRepository.GetEntityState(entityid, on);
        }

        public void RegisterEvent(NewEvent e)
        {
            if (e.Entities == null) throw new Exception("You must specify entities");

            _context.Events.Add(new Provider.EntityFramework.DataModels.Event()
            {
                 EntityList = String.Join(",",e.Entities),
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

            foreach (var agent in _eventAgents)
            {
                foreach (var entity in e.Entities)
                {
                    //agent.OnNewEvent(entity, e.Type, DateTime.Parse(e.On));
                }
            }
        }

        public void RegisterObservation(Observation o)
        {
            var changes = _stateRepository.Track(o);

            foreach (var c in changes)
            {
                foreach (var agent in _eventAgents)
                {
                    var result = agent.OnChange(c.Entity, c.Key, c.OldValue, c.NewValue, c.On);

                    if (result != null)
                    {
                        if (result.NewObservations != null)
                        {
                            foreach (var ob in result.NewObservations) { this.RegisterObservation(ob); }
                        }
                        if (result.NewEvents != null)
                        {
                            foreach (var ev in result.NewEvents) { this.RegisterEvent(ev); }
                        }
                    }
                }
            }
        }

        public IEnumerable<ExistingEvent> FilterEvents(IEnumerable<string> expressions)
        {
            var events = _context.Events.AsQueryable();

            foreach (var expression in expressions)
            {
                events = ParseFilterEventExpressions(expression, events);
            }

            //TODO -- need to optimize this.. we shouldn't have to pull all of them back to do the filtering
            //var contexts = events.ToList().Select(x => new Context()
            //{ 
            //    Event = new Event() { Entities = x.EntityList.Split(',') , On = x.On.ToString("yyyy/MM/dd HH:mm:ss"), Type = x.Type, Id = x.Id.ToString() },
            //    States = x.EntityList.Split(',').Select(y => _stateRepository.GetEntityState(y, x.On.ToString("MM/dd/yyyy HH:mm:ss"))).ToList()
            //});

            //foreach (var expression in expressions)
            //{
            //    contexts = ParseFilterContextExpression(expression, contexts);
            //}

            return events.Select(x => new ExistingEvent() { Entities = x.EntityList.Split(','), On = x.On.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"), Type = x.Type, Id = x.Id.ToString() } );
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

        public IList<StateRange> FilterState(IEnumerable<string> expressions)
        {
            var stateData = _context.TimeAndStates
                .Join(_context.EntityStateKeys, state=> state.Key, key=> key.Key, (state,key) => new {state,key });


            DateTime? afterLimit = null;
            //DateTime? beforeLimit = null;
            //string valueLimit = null;
            //string keyLimit = null;

            var postFilters = new List<Func<StateRange, bool>>();
            var postUpdates = new List<Action<StateRange>>();

            foreach (var expression in expressions)
            {
                if (expression.StartsWith("Entity.State."))
                {
                    var e = expression.Replace("Entity.State.", "");

                    string comparer = "";

                    if (e.Contains("=")) comparer = "=";
                    if (e.Contains("<")) comparer = "<";
                    if (e.Contains(">")) comparer = ">";
                    if (e.Contains(">=")) comparer = ">=";
                    if (e.Contains("<=")) comparer = "<=";

                    var var = e.Split(new string[] { comparer }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                    var value = e.Split(new string[] { comparer }, StringSplitOptions.RemoveEmptyEntries)[1];

                    if(comparer == "=")
                    {
                        stateData = stateData.Where(x => x.state.Key == var && (x.state.Value == value || x.state.PriorValue == value));
                        postFilters.Add(x => x.Key == var && x.Value == value);
                    }

                    if(comparer == "<")
                    {
                        int intVal = Convert.ToInt32(value);
                        stateData = stateData.Where(x => x.state.Key == var && (x.state.NumericValue < intVal || x.state.NumericPriorValue < intVal));
                        postFilters.Add(x => x.Key == var && Convert.ToDecimal(x.Value) < Convert.ToDecimal(value));
                    }

                    if (comparer == "<=")
                    {
                        int intVal = Convert.ToInt32(value);
                        stateData = stateData.Where(x => x.state.Key == var && (x.state.NumericValue <= intVal || x.state.NumericPriorValue <= intVal));
                        postFilters.Add(x => x.Key == var && Convert.ToDecimal(x.Value) <= Convert.ToDecimal(value));
                    }

                    if (comparer == ">")
                    {
                        int intVal = Convert.ToInt32(value);
                        stateData = stateData.Where(x => x.state.Key == var && (x.state.NumericValue > intVal || x.state.NumericPriorValue > intVal));
                        postFilters.Add(x => x.Key == var && Convert.ToDecimal(x.Value) > Convert.ToDecimal(value));
                    }


                    if (comparer == ">=")
                    {
                        int intVal = Convert.ToInt32(value);
                        stateData = stateData.Where(x => x.state.Key == var && (x.state.NumericValue >= intVal || x.state.NumericPriorValue >= intVal));
                        postFilters.Add(x => x.Key == var && Convert.ToDecimal(x.Value) >= Convert.ToDecimal(value));
                    }

                }

                if (expression.StartsWith("On.After"))
                {
                    var e = expression.Replace("On.After", "");
                    var var = e.Split('=')[0];
                    var value = DateTime.Parse(e.Split('=')[1]);
                    stateData = stateData.Where(x => x.state.On > value || ( x.key.LastChange < value && x.state.On == x.key.LastChange ));
                    afterLimit = value;

                    postUpdates.Add((x) =>
                    {
                        if (x.Start < value) { x.Start = value; }
                    }
);
                }

                if (expression.StartsWith("On.Before"))
                {
                    var e = expression.Replace("On.Before", "");
                    var var = e.Split('=')[0];
                    var value = DateTime.Parse(e.Split('=')[1]);
                    stateData = stateData.Where(x => x.state.On < value);

                    postUpdates.Add((x) =>
                        {
                            if (x.End > value || x.End == null) { x.End = value; }
                        }
                    );

                }
            }

            var finalData = stateData.ToList();

            var ret = new List<StateRange>();

            var entities = finalData.Select(x => x.state.Entity).Distinct();

            foreach(var entity in entities)
            {
                foreach (var key in finalData.Where(x => x.state.Entity == entity).Select(x => x.state.Key).Distinct())
                {
                    var ordered = finalData.Where(x => x.state.Entity == entity && x.state.Key == key).OrderBy(x => x.state.On);

                    StateRange c = null;
                    bool isFirst = true;
                    foreach (var o in ordered)
                    {
                        if(isFirst && afterLimit != null && o.state.On > afterLimit && o.state.PriorValue != null)
                        {
                            ret.Add(new StateRange()
                            {
                                Entity = entity,
                                Key = key,
                                Value = o.state.PriorValue,
                                Start = afterLimit.Value,
                                End = o.state.On.AddSeconds(-1)
                            });
                        }

                        if (c == null)
                        {
                            c = new StateRange()
                            {
                                Entity = entity,
                                Key = key,
                                Value = o.state.Value,
                                Start = o.state.On
                            };

                            ret.Add(c);
                        }
                        else
                        {
                            c.End = o.state.On.AddSeconds(-1);

                            c = new StateRange()
                            {
                                Entity = entity,
                                Key = key,
                                Value = o.state.Value,
                                Start = o.state.On
                            };

                            ret.Add(c);

                        }

                        isFirst = false;
                    }
                }
            }

            foreach(var filter in postFilters)
            {
                ret = ret.Where(filter).ToList();
            }

            foreach(var update in postUpdates)
            {
                foreach(var r in ret)
                {
                    update.Invoke(r);
                }
            }

            return ret;
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

        public IList<string> SearchEventTypes(string search)
        {
            return _context.EventTypes
            .Where(x => x.Type.Contains(search))
            .Select(x => x.Type)
            .ToList();
        }

        public IList<string> SearchEntities(string search)
        {
            return _context.TimeAndStates.Select(x => x.Entity)
            .Distinct()
            .Where(x => x.Contains(search))
            .ToList();
        }

        public IList<Cluster> SearchClusters(IEnumerable<string> filterExpressions, IEnumerable<string> clusterExpressions)
        {
            var events = this.FilterEvents(filterExpressions);
            return CreateClusters(clusterExpressions.First(), clusterExpressions.Skip(1), events);
        }

        public IList<Cluster> CreateClusters(string expression, IEnumerable<string> childExpressions, IEnumerable<ExistingEvent> events)
        {
            var clusters = new List<Cluster>();


            if(expression.ToLower().StartsWith("timespan"))
            {

                string comparer = "";
                if (expression.Contains("<=")) comparer = "<=";

                var value = expression.Split(new string[] { comparer }, StringSplitOptions.RemoveEmptyEntries)[1];
                var t = TimeSpan.Parse(value);

                var current = new Cluster() { Events = new List<ExistingEvent>() };
                clusters.Add(current);
                DateTime? lastTime = null;
                
                foreach(var e in events)
                {
                    if(lastTime == null || DateTime.Parse(e.On).Subtract(lastTime.Value) <= t)
                    {
                        current.Events.Add(e);
                        lastTime = DateTime.Parse(e.On);
                    }
                    else
                    {
                        current = new Cluster() { Events = new List<ExistingEvent>() };
                        current.Events.Add(e);
                        lastTime = DateTime.Parse(e.On);
                        clusters.Add(current);
                    }
                }

                foreach(var c in clusters)
                {
                    c.Start = c.Events.First().On;
                    c.End = c.Events.Last().On;        
                }
            }

            if (childExpressions.Count() == 0) return clusters;

            var results = new List<Cluster>();

            foreach (var cluster in clusters)
            {
                results.AddRange(CreateClusters(childExpressions.First(), childExpressions.Skip(1), cluster.Events));
            }

            return results;
        }

    }
}
