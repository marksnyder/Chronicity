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

        [Fact]
        public void EntityLinks_Basic_LinkState()
        {
            var service = new TimeLineService();
            var e1 = new Event()
            {
                On = "2001/01/01",
                Type = "MyEventType1",
                Entity = "E1",
                Observations = new string[] { "Entity.State.MyVal=Hello World" }
            };

            var e2 = new Event()
            {
                On = "2001/01/01",
                Type = "MyEventType2",
                Entity = "E2",
                Observations = new string[] { "Entity.Links.Add=E1" }
            };


            service.RegisterEvent(e1);
            service.RegisterEvent(e2);

            var context = service.FilterEvents(new string[] {"Type=MyEventType2"}).First();

            Assert.Equal("Hello World", context.LinkedState["E1"]["MyVal"]);

            
        }
    }
}
