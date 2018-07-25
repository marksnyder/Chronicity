﻿using Chronicity.Core.Timeline;
using Chronicity.Provider.EntityFramework.DataContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace Chronicity.Provider.EntityFramework.Tests
{
    public class ClusterFilters
    {
        private TimeLineService _service;
        private ChronicityContext _context;

        public ClusterFilters()
        {
            var options = new DbContextOptionsBuilder<ChronicityContext>()
                .UseInMemoryDatabase(databaseName: "ClusterFilters")
                .Options;

            _context = new ChronicityContext(options);

            _service = new TimeLineService(_context);
        }

        [Fact]
        public void ClusterFilter_LessThanOrEqual_Within_Filter()
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
                On = "2001/01/01 01:06",
                Type = "MyEventType1",
                Entities = new[] { "E1" }
            };

            _service.RegisterEvent(e1);
            _service.RegisterEvent(e2);

            var match = _service.ClusterEvents(
                new string[] { "On.After=2001/01/01 01:00" },
                new string[] { "Within <= 0.0:5:0" });


            Assert.Single(match);
            Assert.Equal(new DateTime(2001,1,1,1,1,0).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"), match.First().Start);
            Assert.Equal(new DateTime(2001, 1, 1, 1, 6, 0).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"), match.First().End);
        }

        [Fact]
        public void ClusterFilter_LessThanOrEqual_Within_Filter_Breaks_Properly()
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
                On = "2001/01/01 01:06",
                Type = "MyEventType1",
                Entities = new[] { "E1" }
            };

            var e3 = new NewEvent()
            {
                On = "2001/01/01 01:12",
                Type = "MyEventType1",
                Entities = new[] { "E1" }
            };


            _service.RegisterEvent(e1);
            _service.RegisterEvent(e2);
            _service.RegisterEvent(e3);

            var match = _service.ClusterEvents(
                new string[] { "On.After=2001/01/01 01:00" },
                new string[] { "Within <= 0.0:5:0" });


            Assert.Equal(2,match.Count);
            Assert.Equal(new DateTime(2001, 1, 1, 1, 1, 0).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"), match.First().Start);
            Assert.Equal(new DateTime(2001, 1, 1, 1, 6, 0).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"), match.First().End);

            Assert.Equal(new DateTime(2001, 1, 1, 1, 12, 0).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"), match.Skip(1).First().Start);
            Assert.Equal(new DateTime(2001, 1, 1, 1, 12, 0).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"), match.Skip(1).First().End);
        }


        [Fact]
        public void ClusterFilter_Handles_EmptyResults()
        {
            _context.Database.EnsureDeleted();

            var match = _service.ClusterEvents(
                new string[] { "On.After=2001/01/01 01:00" },
                new string[] { "Within <= 0.0:5:0" });


            Assert.Equal(0, match.Count);
        }



        [Fact]
        public void ClusterFilter_Basic_Sequence_Match()
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
                On = "2001/01/01 01:06",
                Type = "MyEventType2",
                Entities = new[] { "E1" }
            };

            _service.RegisterEvent(e1);
            _service.RegisterEvent(e2);

            var match = _service.ClusterEvents(
                new string[] { "On.After=2001/01/01 01:00" },
                new string[] { "Sequence = [MyEventType1,MyEventType2]" });


            Assert.Single(match);
            Assert.Equal(new DateTime(2001, 1, 1, 1, 1, 0).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"), match.First().Start);
            Assert.Equal(new DateTime(2001, 1, 1, 1, 6, 0).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"), match.First().End);
        }


        [Fact]
        public void ClusterFilter_Sequence_Excludes_outsiders()
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
                On = "2001/01/01 01:06",
                Type = "MyEventType2",
                Entities = new[] { "E1" }
            };
        
            var e3 = new NewEvent()
            {
                On = "2001/01/01 01:07",
                Type = "MyEventType2",
                Entities = new[] { "E1" }
            };

            _service.RegisterEvent(e1);
            _service.RegisterEvent(e2);
            _service.RegisterEvent(e3);

            var match = _service.ClusterEvents(
                new string[] { "On.After=2001/01/01 01:00" },
                new string[] { "Sequence = [MyEventType1,MyEventType2]" });


            Assert.Single(match);
            Assert.Equal(new DateTime(2001, 1, 1, 1, 1, 0).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"), match.First().Start);
            Assert.Equal(new DateTime(2001, 1, 1, 1, 6, 0).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"), match.First().End);
        }


    }
}
