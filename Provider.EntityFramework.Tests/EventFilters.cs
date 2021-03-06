﻿using Chronicity.Provider.EntityFramework;
using System;
using Xunit;
using Chronicity.Core.Timeline;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Chronicity.Provider.EntityFramework.DataContext;

namespace Provider.EntityFramework.Tests
{
    public class EventFilters
    {
        private TimeLineService _service;
        private ChronicityContext _context;

        public EventFilters()
        {
            var options = new DbContextOptionsBuilder<ChronicityContext>()
                .UseInMemoryDatabase(databaseName: "EventFilters")
                .Options;

            _context = new ChronicityContext(options);

            _service = new TimeLineService(_context);
        }



        [Fact]
        public void FilterEvent_BasicStateMatch()
        {
            _context.Database.EnsureDeleted();

            var e1 = new NewEvent()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entities = new string[] { "E1" }
            };

            var o1 = new Observation()
            {
                On = "2001/01/01 01:01",
                Type = "MyObservationType",
                Entity = "E1",
                Expressions = new string[] { "State.MyVal=Hello World" }
            };

            _service.RegisterObservation(o1);
            _service.RegisterEvent(e1);


            var match = _service.SearchEvents(new string[] { "State.MyVal=Hello World" });
            var nonMatch = _service.SearchEvents(new string[] { "State.MyVal=Not The One!" });

            Assert.Single(match);
            Assert.Empty(nonMatch);
        }


        [Fact]
        public void FilterEvent_BasicStateMatch_OutOfOrder()
        {
            _context.Database.EnsureDeleted();

            var e1 = new NewEvent()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entities = new string[] { "E1" }
            };

            var o1 = new Observation()
            {
                On = "2001/01/01 01:01",
                Type = "MyObservationType",
                Entity = "E1",
                Expressions = new string[] { "State.MyVal=Hello World" }
            };

            _service.RegisterEvent(e1);
            _service.RegisterObservation(o1);


            var match = _service.SearchEvents(new string[] { "State.MyVal=Hello World" });
            var nonMatch = _service.SearchEvents(new string[] { "State.MyVal=Not The One!" });

            Assert.Single(match);
            Assert.Empty(nonMatch);
        }


        [Fact]
        public void FilterEvent_TimeMatch_After()
        {
            _context.Database.EnsureDeleted();

            var e1 = new NewEvent()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType1",
                Entities = new [] { "E1" }
            };

            var e2 = new NewEvent()
            {
                On = "2001/01/01 01:02",
                Type = "MyEventType2",
                Entities = new [] { "E1" }
            };

            _service.RegisterEvent(e1);
            _service.RegisterEvent(e2);

            var match = _service.SearchEvents(new string[] { "On>2001/01/01 01:01" });


            Assert.Single(match);
            Assert.Equal("MyEventType2", match.First().Type);
        }


        [Fact]
        public void FilterEvent_TimeMatch_Before()
        {
            _context.Database.EnsureDeleted();

            var e1 = new NewEvent()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType1",
                Entities = new [] { "E1" }
            };

            var e2 = new NewEvent()
            {
                On = "2001/01/01 01:02",
                Type = "MyEventType2",
                Entities = new [] { "E1" }
            };

            _service.RegisterEvent(e1);
            _service.RegisterEvent(e2);

            var match = _service.SearchEvents(new string[] { "On < 2001/01/01 01:02" });


            Assert.Single(match);
            Assert.Equal("MyEventType1", match.First().Type);
        }


        [Fact]
        public void FilterEvent_TimeMatch_Between()
        {
            _context.Database.EnsureDeleted();

            var e1 = new NewEvent()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType1",
                Entities = new [] { "E1" }
            };

            var e2 = new NewEvent()
            {
                On = "2001/01/01 01:03",
                Type = "MyEventType2",
                Entities = new [] { "E1" }
            };

            _service.RegisterEvent(e1);
            _service.RegisterEvent(e2);

            var match = _service.SearchEvents(new string[] { "On > 2001/01/01 01:00", " On < 2001/01/01 01:02" });

            Assert.Single(match);
            Assert.Equal("MyEventType1", match.First().Type);
        }



        [Fact]
        public void FilterEvent_EventTypeMatch()
        {
            _context.Database.EnsureDeleted();

            var e1 = new NewEvent()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entities = new[] { "E1" }
            };

            _service.RegisterEvent(e1);

            var match = _service.SearchEvents(new string[] { "Type=MyEventType" });
            var nonMatch = _service.SearchEvents(new string[] { "Type=Not The One!" });

            Assert.Single(match);
            Assert.Empty(nonMatch);
        }

        [Fact]
        public void FilterEvent_EventMultiTypeMatch()
        {
            _context.Database.EnsureDeleted();

            var e1 = new NewEvent()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType1",
                Entities = new[] { "E1" }
            };

            var e2 = new NewEvent()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType2",
                Entities = new[] { "E1" }
            };

            var e3 = new NewEvent()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType3",
                Entities = new[] { "E1" }
            };

            _service.RegisterEvent(e1);
            _service.RegisterEvent(e2);
            _service.RegisterEvent(e3);

            var match = _service.SearchEvents(new string[] { "Type=[MyEventType1,MyEventType2]" });

            Assert.Equal("MyEventType1",match.First().Type);
            Assert.Equal("MyEventType2", match.Skip(1).First().Type);
            Assert.Equal(2, match.Count());
        }


    }
}
