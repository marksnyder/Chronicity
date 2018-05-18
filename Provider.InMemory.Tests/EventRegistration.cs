using Chronicity.Provider.InMemory;
using System;
using Xunit;
using Chronicity.Core.Events;
using System.Collections.Generic;
using System.Linq;

namespace Provider.InMemory.Tests
{
    public class EventRegistration
    {
        [Fact]
        public void RegisteringEvent_Must_Be_RegisteredEntity()
        {
            var service = new TimeLineService();
            var e = new Event()
            {
                 On = "2001/01/01",
                 Type = "MyEventType"
            };

            Assert.Throws<Exception>(() => service.RegisterEvent(e));
        }


        [Fact]
        public void RegisteringEvent_CanBeRetrieved()
        {
            var service = new TimeLineService();
            var e = new Event()
            {
                On = "2001/01/01",
                Type = "MyEventType",
                Entities = new [] { "E1" }
            };


            service.RegisterEvent(e);

            var contexts = service.FilterEvents(new string[] {});
            
            Assert.Single(contexts);

        }


        [Fact]
        public void RegisteringObservation_IsReturned_In_EventType_List()
        {
            var service = new TimeLineService();

            var e1 = new Event()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entities = new [] { "E1" }
            };

            service.RegisterEvent(e1);

            Assert.Equal("MyEventType", service.GetAllEventTypes().First());
        }


    }
}
