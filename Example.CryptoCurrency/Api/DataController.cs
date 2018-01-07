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
            var model = new DetailView();

            if (id == "undefined") return model;

            var entityId = id.Split('|')[0];
            var time = id.Split('|')[1];

            model.On = time;
            model.StateValues = new List<StateValue>();
            model.Id = entityId;

            var state = _service.GetEntityState(entityId, time);

            ApplyState(model, state, string.Empty);

            var links = _service.GetEntityLinks(entityId, time);

            foreach(var link in links)
            {
                var linkState = _service.GetEntityState(link, time);
                ApplyState(model,linkState, string.Concat(link,"."));
            }


            return model;
        }

        protected void ApplyState(DetailView model, IDictionary<string,string> state, string prefix)
        {
            foreach (var key in state.Keys)
            {

                if (key == "WebLink")
                {
                    model.StateValues.Add(new StateValue()
                    {
                        Key = string.Concat(prefix,key),
                        Value = string.Format("<a target=\"news\" href=\"{0}\">View Link</a>", state[key])
                    });
                }
                else
                {
                    model.StateValues.Add(new StateValue()
                    {
                        Key = string.Concat(prefix, key),
                        Value = state[key]
                    });
                }

            }
        }


        [HttpPost]
        public IEnumerable<VisDataSet> Post([FromBody] FilterRequest datafilters)
        {
            var result = _service.FilterEvents(datafilters.filters);

            return result.Select(MapContent);
        }

        protected VisDataSet MapContent(Chronicity.Core.Events.Context context)
        {

            var result = new VisDataSet();

            if(context.Event.Type == "Headline")
            {
                result.content = string.Format("<span class=\"glyphicon glyphicon-pencil\" style=\"color:#36d4ec\" aria-hidden=\"true\"></span>&nbsp;{0}", context.State["Title"]);
            }
            else if(context.Event.Type == "Price")
            {

                if(context.State["Increase"] == "True")
                {
                    result.content = string.Format("<span class=\"glyphicon glyphicon-triangle-top\" style=\"color:green\" aria-hidden=\"true\"></span>{0} {1}",context.Event.Entity, context.State["Price"]);
                }
                else
                {
                    result.content = string.Format("<span class=\"glyphicon glyphicon-triangle-bottom\" style=\"color:red\" aria-hidden=\"true\"></span>{0} {1}", context.Event.Entity, context.State["Price"]);
                }

            }
            else
            {
                result.content = context.Event.Type;
            }

            result.id = string.Format("{0}|{1}|{2}",context.Event.Entity,context.Event.On,context.GetHashCode());
            result.start = context.Event.On;


            return result;
        }
    }
}