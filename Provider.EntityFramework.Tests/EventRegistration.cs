using Chronicity.Provider.EntityFramework;
using System;
using Xunit;
using Chronicity.Core.Events;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Chronicity.Provider.EntityFramework.DataContext;

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

            _service = new TimeLineService(_context);
        }


        [Fact]
        public void RegisteringEvent_Must_Be_RegisteredEntity()
        {
            _context.Database.EnsureDeleted();

            var e = new Event()
            {
                 On = "2001/01/01",
                 Type = "MyEventType"
            };

            Assert.Throws<Exception>(() => _service.RegisterEvent(e));
        }


        [Fact]
        public void RegisteringEvent_CanBeRetrieved()
        {
            _context.Database.EnsureDeleted();

            var e = new Event()
            {
                On = "2001/01/01",
                Type = "MyEventType",
                Entities = new [] { "E1" }
            };


            _service.RegisterEvent(e);

            var contexts = _service.FilterEvents(new string[] {});
            
            Assert.Single(contexts);

        }


        [Fact]
        public void RegisteringObservation_IsReturned_In_EventType_List()
        {
            _context.Database.EnsureDeleted();

            var e1 = new Event()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entities = new [] { "E1" }
            };

            _service.RegisterEvent(e1);

            Assert.Equal("MyEventType", _service.GetAllEventTypes().First());
        }


    }
}
