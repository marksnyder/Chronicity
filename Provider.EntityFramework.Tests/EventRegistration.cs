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
    public class EventRegistration
    {
        private TimeLineService _service;
        private ChronicityContext _context;

        public EventRegistration()
        {
            var options = new DbContextOptionsBuilder<ChronicityContext>()
                .UseInMemoryDatabase(databaseName: "EventRegistration")
                .Options;

            _context = new ChronicityContext(options);
        }


        [Fact]
        public void RegisteringEvent_Must_Be_RegisteredEntity()
        {
            _context.Database.EnsureDeleted();
            var service = new TimeLineService(_context);

            var e = new Event()
            {
                 On = "2001/01/01",
                 Type = "MyEventType"
            };

            Assert.Throws<Exception>(() => service.RegisterEvent(e));
        }

        [Fact]
        public void RegisteringEvent_Fires_EventAgent()
        {
            _context.Database.EnsureDeleted();

            var agentMock = new Mock<IEventAgent>();

            var service = new TimeLineService(_context, new List<IEventAgent> () { agentMock.Object });

            var e = new Event()
            {
                On = "2001/01/01",
                Type = "MyEventType",
                Entities = new[] { "MyEntity" }
            };

            service.RegisterEvent(e);

            agentMock.Verify(x => x.OnNewEvent("MyEntity", "MyEventType", new DateTime(2001,1,1)));
        }

        [Fact]
        public void RegisteringEvent_Multiple_Entities_Fires_EventAgent()
        {
            _context.Database.EnsureDeleted();

            var agentMock = new Mock<IEventAgent>();

            var service = new TimeLineService(_context, new List<IEventAgent>() { agentMock.Object });

            var e = new Event()
            {
                On = "2001/01/01",
                Type = "MyEventType",
                Entities = new[] { "MyEntity", "MyEntity2" }
            };

            service.RegisterEvent(e);

            agentMock.Verify(x => x.OnNewEvent("MyEntity", "MyEventType", new DateTime(2001, 1, 1)));
            agentMock.Verify(x => x.OnNewEvent("MyEntity2", "MyEventType", new DateTime(2001, 1, 1)));
        }

        [Fact]
        public void RegisteringEvent_Multiple_Providers_Fires_EventAgent()
        {
            _context.Database.EnsureDeleted();

            var agentMock = new Mock<IEventAgent>();
            var agentMock2 = new Mock<IEventAgent>();

            var service = new TimeLineService(_context, new List<IEventAgent>() { agentMock.Object, agentMock2.Object });

            var e = new Event()
            {
                On = "2001/01/01",
                Type = "MyEventType",
                Entities = new[] { "MyEntity" }
            };

            service.RegisterEvent(e);

            agentMock.Verify(x => x.OnNewEvent("MyEntity", "MyEventType", new DateTime(2001, 1, 1)));
            agentMock2.Verify(x => x.OnNewEvent("MyEntity", "MyEventType", new DateTime(2001, 1, 1)));
        }

        [Fact]
        public void RegisteringEvent_CanBeRetrieved()
        {
            _context.Database.EnsureDeleted();
            var service = new TimeLineService(_context);

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
            _context.Database.EnsureDeleted();
            var service = new TimeLineService(_context);

            var e1 = new Event()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entities = new [] { "E1" }
            };

            service.RegisterEvent(e1);

            Assert.Equal("MyEventType", service.SearchEventTypes("My").First());
        }


    }
}
