using Chronicity.Core.Events;
using Chronicity.Provider.EntityFramework;
using Chronicity.Provider.EntityFramework.DataContext;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

namespace Provider.EntityFramework.Tests
{
    public class EntityRegistration
    {
        private TimeLineService _service;

        public EntityRegistration()
        {
            var options = new DbContextOptionsBuilder<ChronicityContext>()
                .UseInMemoryDatabase(databaseName: "EntityRegistration")
                .Options;

            _service = new TimeLineService(new ChronicityContext(options));
        }


        [Fact]
        public void Entity_State_Tracked()
        {
            var e1 = new NewEvent()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entities = new [] { "E1" }
            };

            var o1 = new Observation()
            {
                On = "2001/01/01 01:01",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyVal=Hello World" }
            };

            var e2 = new NewEvent()
            {
                On = "2001/01/01 01:02",
                Type = "MyEventType",
                Entities = new[] { "E1" }
            };

            var o2 = new Observation()
            {
                On = "2001/01/01 01:02",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyVal=Hello World Again" }
            };



            _service.RegisterObservation(o1);
            _service.RegisterEvent(e1);
            _service.RegisterObservation(o2);
            _service.RegisterEvent(e2);

            var state1 = _service.GetEntityState("E1","2001 /01/01 01:01");
            var state2 = _service.GetEntityState("E1", "2001/01/01 01:02");
            var state3 = _service.GetEntityState("BADID", "2001/01/01 01:02");

            Assert.Equal("Hello World", state1["MyVal"]);
            Assert.Equal("Hello World Again", state2["MyVal"]);
            Assert.False(state3.ContainsKey("MyVal"));
        }

        [Fact]
        public void Entity_State_Tracked_BetweenEvents()
        {
            var e1 = new NewEvent()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entities = new [] { "E1" }
            };

            var o1 = new Observation()
            {
                On = "2001/01/01 01:01",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyVal=Hello World" }
            };

            var e2 = new NewEvent()
            {
                On = "2001/01/01 01:03",
                Type = "MyEventType",
                Entities = new[] { "E1" }
            };

            var o2 = new Observation()
            {
                On = "2001/01/01 01:03",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyVal=Hello World Again" }
            };


            _service.RegisterEvent(e1);
            _service.RegisterObservation(o1);
            _service.RegisterEvent(e2);
            _service.RegisterObservation(o2);

            var state1 = _service.GetEntityState("E1", "2001 /01/01 01:02");
            var state2 = _service.GetEntityState("E1", "2001/01/01 01:04");


            Assert.Equal("Hello World", state1["MyVal"]);
            Assert.Equal("Hello World Again", state2["MyVal"]);

        }
    }
}
