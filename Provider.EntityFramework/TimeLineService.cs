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

        public IEnumerable<ExistingEvent> SearchEvents(IEnumerable<string> expressions)
        {
            var events = _context.Events.AsQueryable();

            foreach (var expression in expressions)
            {
                events = ParseFilterEventExpressions(expression, events);
            }

            var stateExpressions = expressions.Where(x => x.StartsWith("State."));

            if (stateExpressions.Count() > 0)
            {
                var entities = events.SelectMany(x => x.EntityList.Split(',')).Distinct().ToArray();
                var stateRanges = FilterState(stateExpressions, entities);

                var filteredEvents = new List<Event>();

                foreach (var e in events)
                {
                    var enlist = e.EntityList.Split(',');
                    var relState = stateRanges.Where(x => enlist.Contains(x.Entity) && x.Start <= e.On && (x.End == null || x.End >= x.End.Value));
                    if (relState.Count() > 0) filteredEvents.Add(e);
                }

                events = filteredEvents.AsQueryable();

            }

            return events.Select(x => new ExistingEvent() { Entities = x.EntityList.Split(','), On = x.On.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"), Type = x.Type, Id = x.Id.ToString() } );
        }


        private IQueryable<DataModels.Event> ParseFilterEventExpressions(string expression, IQueryable<DataModels.Event> events)
        {
            var ret = events;

            string comparer = "";

            if (expression.Contains("=")) comparer = "=";
            if (expression.Contains("<")) comparer = "<";
            if (expression.Contains(">")) comparer = ">";
            if (expression.Contains(">=")) comparer = ">=";
            if (expression.Contains("<=")) comparer = "<=";

            var action = expression.Split(comparer.ToCharArray())[0].Trim();
            var value = expression.Split(comparer.ToCharArray())[1].Trim();

            if (expression.StartsWith("Type"))
            {
                List<string> includeValues;

                if(value.Contains("["))
                {
                    includeValues = value.Replace("[","" ).Replace("]", "").Split(',').ToList();
                }
                else
                {
                    includeValues = new List<string>() { value };
                }

                ret = ret.Where(x => includeValues.Contains(x.Type));
            }
            else if(action == "On")
            {
                if(comparer == "=") ret = ret.Where(x => x.On == DateTime.Parse(value));
                if(comparer == ">=") ret = ret.Where(x => x.On >= DateTime.Parse(value));
                if(comparer == ">") ret = ret.Where(x => x.On > DateTime.Parse(value));
                if(comparer == "<=") ret = ret.Where(x => x.On <= DateTime.Parse(value));
                if(comparer == "<") ret = ret.Where(x => x.On < DateTime.Parse(value));
            }

            return ret.AsQueryable();
        }

        public IList<StateRange> SearchState(IEnumerable<string> expressions)
        {
            return FilterState(expressions, null);
        }

        private IList<StateRange> FilterState(IEnumerable<string> expressions, string[] entityLimit)
        {
            var stateData = _context.TimeAndStates
            .Join(_context.EntityStateKeys, state => state.Key, key => key.Key, (state, key) => new { state, key });

            if (entityLimit != null)
            {
                stateData = stateData.Where(x => entityLimit.Contains(x.key.Entity));
            }


            DateTime? afterLimit = null;

            var postFilters = new List<Func<StateRange, bool>>();
            var postUpdates = new List<Action<StateRange>>();

            foreach (var expression in expressions)
            {
                if (expression.StartsWith("State."))
                {
                    var e = expression.Replace("State.", "");

                    string comparer = "";

                    if (e.Contains("=")) comparer = "=";
                    if (e.Contains("<")) comparer = "<";
                    if (e.Contains(">")) comparer = ">";
                    if (e.Contains(">=")) comparer = ">=";
                    if (e.Contains("<=")) comparer = "<=";

                    var var = e.Split(new string[] { comparer }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                    var value = e.Split(new string[] { comparer }, StringSplitOptions.RemoveEmptyEntries)[1];

                    if (comparer == "=")
                    {
                        stateData = stateData.Where(x => x.state.Key == var && (x.state.Value == value || x.state.PriorValue == value));
                        postFilters.Add(x => x.Key == var && x.Value == value);
                    }

                    if (comparer == "<")
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

                if (expression.StartsWith("After"))
                {
                    var e = expression.Replace("After", "");
                    var var = e.Split('=')[0];
                    var value = DateTime.Parse(e.Split('=')[1]);
                    stateData = stateData.Where(x => x.state.On > value || (x.key.LastChange < value && x.state.On == x.key.LastChange));
                    afterLimit = value;

                    postUpdates.Add((x) =>
                    {
                        if (x.Start < value) { x.Start = value; }
                    }
);
                }

                if (expression.StartsWith("Before"))
                {
                    var e = expression.Replace("Before", "");
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

            foreach (var entity in entities)
            {
                foreach (var key in finalData.Where(x => x.state.Entity == entity).Select(x => x.state.Key).Distinct())
                {
                    var ordered = finalData.Where(x => x.state.Entity == entity && x.state.Key == key).OrderBy(x => x.state.On);

                    StateRange c = null;
                    bool isFirst = true;
                    foreach (var o in ordered)
                    {
                        if (isFirst && afterLimit != null && o.state.On > afterLimit && o.state.PriorValue != null)
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

            foreach (var filter in postFilters)
            {
                ret = ret.Where(filter).ToList();
            }

            foreach (var update in postUpdates)
            {
                foreach (var r in ret)
                {
                    update.Invoke(r);
                }
            }

            return ret;
        }

        private IEnumerable<Context> ParseFilterContextExpression(string expression, IEnumerable<Context> contexts)
        {
            var ret = contexts;

            if (expression.StartsWith("State."))
            {
                var e = expression.Replace("State.", "");
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

        public IList<Cluster> ClusterEvents(IEnumerable<string> filterExpressions, IEnumerable<string> clusterExpressions)
        {
            var events = this.SearchEvents(filterExpressions);

            if (events.Count() < 1 ) return new List<Cluster>();

            var eventClusters = events.Select(x => new Cluster()
            {
                 End = x.On,
                 Start = x.On,
                 Entities = x.Entities,
                 Type = x.Type
            });

            var layers = new Dictionary<int,List<string>>();

            foreach(var exp in clusterExpressions)
            {
                int layer = 1;
                string expClean = exp;

                if(exp.Contains("|"))
                {
                    var layerSplit = exp.Split('|');
                    layer = int.Parse(layerSplit[0]);
                    expClean = string.Concat(layerSplit[1], "|", layerSplit[2]);
                }

                if(!layers.ContainsKey(layer))
                {
                    layers[layer] = new List<string>();
                }

                layers[layer].Add(expClean);
            }

            IEnumerable<Cluster> clusters = eventClusters;

            foreach(var key in layers.Keys.OrderBy(x => x))
            {
                var layerClusters = new List<Cluster>();

                foreach(var exp in layers[key])
                {
                    layerClusters.AddRange(CreateClusters(exp, clusters.OrderBy(x => x.Start)));
                }

                clusters = layerClusters;
            }

            return clusters.ToList();
        }

        public IList<Cluster> CreateClusters(string expression,  IEnumerable<Cluster> events)
        {
            var clusters = new List<Cluster>();

            string typedExpression = string.Empty;

            if(expression.Contains("|"))
            {
                var typeSplit = expression.Split('|');
                typedExpression = typeSplit[0].Trim();
                expression = typeSplit[1].Trim();
            }


            if(expression.ToLower().StartsWith("within"))
            {

                string comparer = "";
                if (expression.Contains("<=")) comparer = "<=";

                var value = expression.Split(new string[] { comparer }, StringSplitOptions.RemoveEmptyEntries)[1];
                var t = TimeSpan.Parse(value);

                var current = new Cluster() { Entities = new List<string>() };
                clusters.Add(current);
                DateTime? lastTime = null;
                
                foreach(var e in events)
                {
                    if(lastTime == null || DateTime.Parse(e.Start).Subtract(lastTime.Value) <= t)
                    {
                        current.Count++;
                        lastTime = DateTime.Parse(e.End);
                        if (String.IsNullOrEmpty(current.Start) || DateTime.Parse(e.Start) < DateTime.Parse(current.Start)) current.Start = e.Start;
                        if (String.IsNullOrEmpty(current.End) || DateTime.Parse(e.End) > DateTime.Parse(current.End)) current.End = e.End;
                        current.Entities = current.Entities.Concat(e.Entities).Distinct();

                    }
                    else
                    {
                        current = new Cluster() { Entities = new List<string>() };
                        current.Count++;
                        lastTime = DateTime.Parse(e.End);
                        if (String.IsNullOrEmpty(current.Start) || DateTime.Parse(e.Start) < DateTime.Parse(current.Start)) current.Start = e.Start;
                        if (String.IsNullOrEmpty(current.End) || DateTime.Parse(e.End) > DateTime.Parse(current.End)) current.End = e.End;
                        current.Entities = current.Entities.Concat(e.Entities).Distinct();
                        clusters.Add(current);
                    }
                }
            }
            else if(expression.ToLower().StartsWith("sequence"))
            {
                var sequence = expression.Split('=')[1].Trim().Replace("[","").Replace("]","").Split(',');

                var eventStack = new List<Cluster>();
                int seqPosition = 0;

                foreach(var e in events.OrderBy(x => x.Start))
                {
                    if(e.Type == sequence[seqPosition])
                    {
                        if(seqPosition >= sequence.Count() -1)
                        {
                            // Pattern matches - add cluster and reset
                            eventStack.Add(e);
                            clusters.Add(new Cluster()
                            {
                                Count = eventStack.Count,
                                Start = eventStack.Min(x => DateTime.Parse(x.Start)).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                                End = eventStack.Max(x => DateTime.Parse(x.End)).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                                Entities = eventStack.SelectMany(x => x.Entities).Distinct().ToList()
                            });
                            seqPosition = 0;
                            eventStack.Clear();
                        }
                        else
                        {
                        // Match - not there yet though
                            eventStack.Add(e);
                            seqPosition++;
                        }
                    }
                    else
                    {
                        // Not a match - reset everything
                        eventStack = new List<Cluster>();

                        if(e.Type == sequence[0])
                        {
                            // Wasn't a match - but it does match the first so start there
                            eventStack.Add(e);
                            seqPosition = 1;
                        }
                        else
                        {
                            // Start over 
                            seqPosition = 0;
                        }


                    }
                }

            }
            foreach (var c in clusters)
            {
                c.Type = typedExpression;
            }


            return clusters;
        }

    }
}
