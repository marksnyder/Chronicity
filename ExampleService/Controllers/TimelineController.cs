using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronicity.Core;
using Chronicity.Core.Entity;
using Chronicity.Core.Events;
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
        public void RegisterEvent(Event e)
        {
            _service.RegisterEvent(e);
        }

        [HttpGet, Route("/RegisterObservation")]
        public void RegisterObservation(Observation o)
        {
            _service.RegisterObservation(o);
        }

        [HttpGet, Route("/FilterEvents")]
        public IEnumerable<Event> FilterEvents(IEnumerable<string> expressions)
        {
            return _service.FilterEvents(expressions);
        }

        [HttpGet, Route("/FilterState")]
        public IEnumerable<StateRange> FilterState(IEnumerable<string> expressions)
        {
            return _service.FilterState(expressions);
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

    }
}