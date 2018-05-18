using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronicity.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DataBrowser.Api
{
    [Produces("application/json")]
    [Route("api/EventType")]
    public class EventTypeController : Controller
    {
        public ITimelineService _service;


        public EventTypeController(ITimelineService service)
        {
            _service = service;
        }

        [HttpGet]

        public List<string> Get()
        {
            return _service.GetAllEventTypes().ToList();
        }
    }
}