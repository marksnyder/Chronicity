using Chronicity.Core.Events;
using Chronicity.Provider.InMemory;
using System;
using Xunit;

namespace Provider.InMemory.Tests
{
    public class EntityRegistration
    {

        [Fact]
        public void Entity_State_Tracked()
        {
            var e1 = new Event()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entity = "E1",
                Observations = new string[] { "Entity.State.MyVal=Hello World" }
            };

            var e2 = new Event()
            {
                On = "2001/01/01 01:02",
                Type = "MyEventType",
                Entity = "E1",
                Observations = new string[] { "Entity.State.MyVal=Hello World Again" }
            };


            var service = new TimeLineService();


            service.RegisterEvent(e1);
            service.RegisterEvent(e2);

            var state1 = service.GetEntityState("E1","2001 /01/01 01:01");
            var state2 = service.GetEntityState("E1", "2001/01/01 01:02");
            var state3 = service.GetEntityState("BADID", "2001/01/01 01:02");

            Assert.Equal("Hello World", state1["MyVal"]);
            Assert.Equal("Hello World Again", state2["MyVal"]);
            Assert.False(state3.ContainsKey("MyVal"));
        }

        [Fact]
        public void Entity_State_Tracked_BetweenEvents()
        {
            var e1 = new Event()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entity = "E1",
                Observations = new string[] { "Entity.State.MyVal=Hello World" }
            };

            var e2 = new Event()
            {
                On = "2001/01/01 01:03",
                Type = "MyEventType",
                Entity = "E1",
                Observations = new string[] { "Entity.State.MyVal=Hello World Again" }
            };


            var service = new TimeLineService();


            service.RegisterEvent(e1);
            service.RegisterEvent(e2);

            var state1 = service.GetEntityState("E1", "2001 /01/01 01:02");
            var state2 = service.GetEntityState("E1", "2001/01/01 01:04");


            Assert.Equal("Hello World", state1["MyVal"]);
            Assert.Equal("Hello World Again", state2["MyVal"]);

        }
    }
}
