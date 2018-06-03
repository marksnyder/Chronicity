using Chronicity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chronicity.Service.EventAgents
{
    public class ExampleEventAgent : IEventAgent
    {
        private ITimelineService _service;

        public ExampleEventAgent(ITimelineService service)
        {
            _service = service;
        }

        void IEventAgent.OnEntityStateChange(string entity, string key, string priorValue, string newValue, DateTime on)
        {

            // TEMP INCREASE

            if (key == "temp")
            {
                if (Convert.ToInt32(newValue) > Convert.ToInt32(priorValue))
                {
                    // Temp Increase
                    _service.RegisterEvent(new Core.Events.Event()
                    {
                        Entities = new[] { entity },
                        On = on.ToString("MM/dd/yyyy HH:mm:ss"),
                        Type = "Temp Increase"
                    });
                }

                if (Convert.ToInt32(newValue) < Convert.ToInt32(priorValue))
                {
                    // Temp Decreease
                    _service.RegisterEvent(new Core.Events.Event()
                    {
                        Entities = new[] { entity },
                        On = on.ToString("MM/dd/yyyy HH:mm:ss"),
                        Type = "Temp Decrease"
                    });
                }
            }

            //// PROXA
            if (key == "proxa")
            {
                if (Convert.ToInt32(newValue) > 40 && Convert.ToInt32(priorValue) <= 40)
                {
                    // Bird departed
                    _service.RegisterEvent(new Core.Events.Event()
                    {
                        Entities = new[] { entity },
                        On = on.ToString("MM/dd/yyyy HH:mm:ss"),
                        Type = "Bird Departed"
                    });
                }

                if (Convert.ToInt32(newValue) < 40 && Convert.ToInt32(priorValue) >= 40)
                {
                    // Bird arrived
                    _service.RegisterEvent(new Core.Events.Event()
                    {
                        Entities = new[] { entity },
                        On = on.ToString("MM/dd/yyyy HH:mm:ss"),
                        Type = "Bird Arrived"
                    });
                }

            }
        }

        void IEventAgent.OnNewEvent(string entity, string type, DateTime on)
        {
        }
    }
}
