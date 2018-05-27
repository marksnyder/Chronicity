using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronicity.Core;
using Chronicity.Core.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    [Produces("application/json")]
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
        public IEnumerable<Context> FilterEvents(IEnumerable<string> expressions)
        {
            return _service.FilterEvents(expressions);
        }

    }
}