using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronicity.Core;
using Chronicity.Core.Reaction;
using Chronicity.Core.Timeline;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    [Produces("application/json")]
    [EnableCors("ChronicityPolicy")]
    public class TimelineController : Controller
    {
        private ITimelineService _service;

        public TimelineController(ITimelineService service)
        {
            _service = service;
        }

        [HttpGet, Route("/GetEntityState")]
        public IDictionary<string, string> GetEntityState(string entityid, string on)
        {
            return _service.GetEntityState(entityid, on);
        }

        [HttpGet, Route("/RegisterEvent")]
        public void RegisterEvent(NewEvent e)
        {
            _service.RegisterEvent(e);
        }

        [HttpGet, Route("/RegisterObservation")]
        public void RegisterObservation(Observation o)
        {
            _service.RegisterObservation(o);
        }

        [HttpGet, Route("/SearchEvents")]
        public IEnumerable<ExistingEvent> SearchEvents(IEnumerable<string> expressions)
        {
            return _service.SearchEvents(expressions);
        }

        [HttpGet, Route("/SearchState")]
        public IEnumerable<StateRange> SearchState(IEnumerable<string> expressions)
        {
            return _service.SearchState(expressions);
        }

        [HttpGet, Route("/SearchEventTypes")]
        public IList<string> SearchEventTypes(string search)
        {
            return _service.SearchEventTypes(search);
        }

        [HttpGet, Route("/SearchEntities")]
        public IList<string> SearchEntities(string search)
        {
            return _service.SearchEntities(search);
        }

        [HttpGet, Route("/ClusterEvents")]
        public IList<Cluster> ClusterEvents(IEnumerable<string> filterExpressions, IEnumerable<string> clusterExpressions)
        {
            return _service.ClusterEvents(filterExpressions, clusterExpressions);
        }

    }
}