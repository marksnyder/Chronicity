using Chronicity.Provider.InMemory;
using System;
using Xunit;
using Chronicity.Core.Events;
using System.Collections.Generic;
using Chronicity.Core.Entities;
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
                 Id = "1",
                 On = new DateTime(2000,1,1),
                 Type = "MyEventType",
                 Changes = new Chronicity.Core.Entities.State()
                 {
                     AttributeChanges = new List<Chronicity.Core.Entities.Attribute>(),
                     RelationshipChanges = new List<Relationship>()
                 }
            };

            Assert.Throws<Exception>(() => service.RegisterEvent(e));
        }


        [Fact]
        public void RegisteringEvent_CanBeRetrieved()
        {
            var service = new TimeLineService();
            var e = new Event()
            {
                Id = "1",
                On = new DateTime(2000, 1, 1),
                Type = "MyEventType",
                EntityId = "E1",
                Changes = new Chronicity.Core.Entities.State()
                {
                    AttributeChanges = new List<Chronicity.Core.Entities.Attribute>(),
                    RelationshipChanges = new List<Relationship>()
                }
            };

            service.RegisterEntity("E1", "MyEntityType");
            service.RegisterEvent(e);

            var contexts = service.FilterEvents(new Filter()
            {
                EntityId = new List<string>() { "E1" }
            });
            
            Assert.Single(contexts);

        }
    }
}
