using Chronicity.Core.Events;
using Chronicity.Provider.InMemory;
using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace Provider.InMemory.Tests
{
    public class EntityLinking
    {
        [Fact]
        public void EntityLinks_Basic_LinkEntities()
        {
            var service = new TimeLineService();
            var e = new Event()
            {
                On = "2001/01/01",
                Type = "MyEventType",
                Entity = "E1",
                Observations = new string[] { "Entity.Links.Add=E2" }
            };

            service.RegisterEvent(e);

            var contexts = service.FilterEvents(new string[] { });

            Assert.Contains("E2", contexts.First().Links);
        }

    }
}
