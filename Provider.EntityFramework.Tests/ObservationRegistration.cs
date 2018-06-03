using Chronicity.Provider.EntityFramework;
using System;
using Xunit;
using Chronicity.Core.Events;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Chronicity.Provider.EntityFramework.DataContext;
using Chronicity.Core;
using Moq;

namespace Provider.EntityFramework.Tests
{
    public class ObservationRegistration
    {
        private ChronicityContext _context;

        public ObservationRegistration()
        {
            var options = new DbContextOptionsBuilder<ChronicityContext>()
                .UseInMemoryDatabase(databaseName: "ObservationRegistration")
                .Options;

            _context = new ChronicityContext(options);

        }



        [Fact]
        public void RegisteringObservation_StateIsSet()
        {
            _context.Database.EnsureDeleted();
            var service = new TimeLineService(_context);

            var o = new Observation()
            {
                On = "2001/01/01",
                Type = "MyEventType",
                Entity = "E1" ,
                Expressions = new[] { "Entity.State.MyVal=Hello World" }
            };

            service.RegisterObservation(o);

            Assert.Equal("Hello World", service.GetEntityState("E1", "2001/01/01")["MyVal"]);
        }

        [Fact]
        public void RegisteringObservation_StateIsMerged()
        {
            _context.Database.EnsureDeleted();
            var service = new TimeLineService(_context);

            var o1 = new Observation()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyVal=Hello World" }
            };

            var o2 = new Observation()
            {
                On = "2001/01/01 01:02",
                Type = "MyEventType",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyNextVal=Hello World Again" }
            };

            service.RegisterObservation(o1);
            service.RegisterObservation(o2);

            Assert.Equal("Hello World", service.GetEntityState("E1", "2001/01/01 01:02")["MyVal"]);
            Assert.Equal("Hello World Again", service.GetEntityState("E1", "2001/01/01 01:02")["MyNextVal"]);
        }

        [Fact]
        public void RegisteringObservation_StateIsMerged_Chronologically()
        {
            _context.Database.EnsureDeleted();
            var service = new TimeLineService(_context);

            var o1 = new Observation()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyVal=Hello World" }
            };

            var o2 = new Observation()
            {
                On = "2001/01/01 01:02",
                Type = "MyEventType",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyNextVal=Hello World Again" }
            };

            service.RegisterObservation(o1);
            service.RegisterObservation(o2);

            Assert.False(service.GetEntityState("E1", "2001/01/01 01:01").ContainsKey("MyNextVal"));
        }




        [Fact]
        public void RegisteringObservation_StateIsMerged_Reverse_Chronologically()
        {
            _context.Database.EnsureDeleted();
            var service = new TimeLineService(_context);

            var o1 = new Observation()
            {
                On = "2001/01/01 01:03",
                Type = "MyEventType",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyNextVal=3" }
            };

            var o2 = new Observation()
            {
                On = "2001/01/01 01:02",
                Type = "MyEventType",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyNextVal=2" }
            };

            var o3 = new Observation()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyNextVal=1" }
            };


            service.RegisterObservation(o1);
            service.RegisterObservation(o2);
            service.RegisterObservation(o3);

            Assert.Equal("1",service.GetEntityState("E1", "2001/01/01 01:01")["MyNextVal"]);
            Assert.Equal("2",service.GetEntityState("E1", "2001/01/01 01:02")["MyNextVal"]);
            Assert.Equal("3",service.GetEntityState("E1", "2001/01/01 01:03")["MyNextVal"]);
        }

        [Fact]
        public void RegisteringObservation_StateIsMerged_Simultaneously()
        {
            _context.Database.EnsureDeleted();
            var service = new TimeLineService(_context);

            var o1 = new Observation()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyVal=Hello World" }
            };

            var o2 = new Observation()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyNextVal=Hello World Again" }
            };


            service.RegisterObservation(o1);
            service.RegisterObservation(o2);

            Assert.Equal("Hello World", service.GetEntityState("E1", "2001/01/01 01:01")["MyVal"]);
            Assert.Equal("Hello World Again", service.GetEntityState("E1", "2001/01/01 01:01")["MyNextVal"]);
        }

        [Fact]
        public void RegisteringObservation_StateIsMerged_MultipleExpressions()
        {
            _context.Database.EnsureDeleted();
            var service = new TimeLineService(_context);

            var o1 = new Observation()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyVal=Hello World", "Entity.State.MyNextVal=Hello World Again" }
            };

            service.RegisterObservation(o1);

            Assert.Equal("Hello World", service.GetEntityState("E1", "2001/01/01 01:01")["MyVal"]);
            Assert.Equal("Hello World Again", service.GetEntityState("E1", "2001/01/01 01:01")["MyNextVal"]);
        }

        [Fact]
        public void RegisteringObservation_StateIsOverwritten()
        {
            _context.Database.EnsureDeleted();
            var service = new TimeLineService(_context);

            var o1 = new Observation()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyVal=Hello World" }
            };

            var o2 = new Observation()
            {
                On = "2001/01/01 01:02",
                Type = "MyEventType",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyNextVal=Hello World Again" }
            };


            service.RegisterObservation(o1);
            service.RegisterObservation(o2);

            var contexts = service.FilterEvents(new string[] { }).OrderByDescending(x => x.On);

            Assert.Equal("Hello World Again", service.GetEntityState("E1", "2001/01/01 01:02")["MyNextVal"]);
        }


        [Fact]
        public void RegisteringObservation_Fires_EventAgent()
        {
            _context.Database.EnsureDeleted();
            var agentMock = new Mock<IEventAgent>();
            var service = new TimeLineService(_context);
            service.RegisterAgent(agentMock.Object);

            var o1 = new Observation()
            {
                On = "2001/01/01",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyVal=Hello World" }
            };


            var o2 = new Observation()
            {
                On = "2001/01/02",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyVal=Hello World Again" }
            };


            service.RegisterObservation(o1);
            service.RegisterObservation(o2);


            agentMock.Verify(x => x.OnEntityStateChange("E1", "MyVal", "Hello World", "Hello World Again", new DateTime(2001,1,2)));
        }

        [Fact]
        public void RegisteringObservation_Fires_EventAgent_Out_Of_Order()
        {
            _context.Database.EnsureDeleted();
            var agentMock = new Mock<IEventAgent>();
            var service = new TimeLineService(_context);
            service.RegisterAgent(agentMock.Object);

            var o1 = new Observation()
            {
                On = "2001/01/01",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyVal=Hello World" }
            };


            var o2 = new Observation()
            {
                On = "2001/01/02",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyVal=Hello World Again" }
            };


            service.RegisterObservation(o2);
            service.RegisterObservation(o1);


            agentMock.Verify(x => x.OnEntityStateChange("E1", "MyVal", "Hello World", "Hello World Again", new DateTime(2001, 1, 2)));
        }
    }
}
