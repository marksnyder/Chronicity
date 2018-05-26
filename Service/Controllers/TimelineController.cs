using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    [Produces("application/json")]
    [Route("v1/Timeline")]
    public class TimelineController : Controller
    {
        [HttpGet]
        public IDictionary<string, string> GetEntityState(string entityid, string on)
        {
            return null;
        }
    }
}