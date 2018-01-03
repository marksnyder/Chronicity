using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronicity.Core;
using Example.CatsAndDogs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Chronicity.Core.Events;

namespace Example.CatsAndDogs.Api
{
    [Produces("application/json")]
    [Route("api/Data")]
    public class DataController : Controller
    {
        public ITimelineService _service;


        public DataController(ITimelineService service)
        {
            _service = service;
        }

        [HttpGet]
        public DetailView Get(string id)
        {
            var entityId = id.Split('|')[0];
            var time = id.Split('|')[1];

            var model = new DetailView();
            model.On = time;
            model.StateValues = new List<StateValue>();
            model.Id = entityId;

            var state = _service.GetEntityState(entityId, time);  
            
            foreach(var key in state.Keys)
            {
                model.StateValues.Add(new StateValue()
                {
                    Key = key,
                    Value = state[key]
                });
            }

            return model;
        }


        [HttpPost]
        public IEnumerable<VisDataSet> Post([FromBody] FilterRequest datafilters)
        {
            var result = _service.FilterEvents(datafilters.filters).Take(500);

            return result.Select(MapContent);
        }

        protected VisDataSet MapContent(Chronicity.Core.Events.Context context)
        {

            var result = new VisDataSet();

            result.id = string.Format("{0}|{1}",context.Event.Entity,context.Event.On);
            result.start = context.Event.On;
            result.content = string.Format("{1} - {0}", context.State["Name"], context.Event.Type );


            return result;
        }
    }
}