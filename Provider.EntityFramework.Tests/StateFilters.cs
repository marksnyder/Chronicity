using Chronicity.Provider.EntityFramework;
using System;
using Xunit;
using Chronicity.Core.Timeline;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Chronicity.Provider.EntityFramework.DataContext;
using Chronicity.Core;
using Moq;

namespace Chronicity.Provider.EntityFramework.Tests
{
    public class StateFilters
    {
        private ChronicityContext _context;

        public StateFilters()
        {
            var options = new DbContextOptionsBuilder<ChronicityContext>()
                .UseInMemoryDatabase(databaseName: "StateFilters")
                .Options;

            _context = new ChronicityContext(options);

        }

        [Fact]
        public void FilterByKey_ReturnsResults()
        {
            _context.Database.EnsureDeleted();
            var service = new TimeLineService(_context);

            var o = new Observation()
            {
                On = "2001/01/01",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=Hello World" }
            };

            var o2 = new Observation()
            {
                On = "2001/01/02",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=More" }
            };

            service.RegisterObservation(o);
            service.RegisterObservation(o2);

            var result = service.SearchState(new[] { "State.MyVal=Hello World" }).First();

            Assert.Equal("E1", result.Entity);
            Assert.Equal(new DateTime(2001, 1, 1), result.Start);
            Assert.Equal(new DateTime(2001, 1, 2).AddSeconds(-1), result.End);
        }

        [Fact]
        public void FilterByKey_Returns_Only_Match_Results()
        {
            _context.Database.EnsureDeleted();
            var service = new TimeLineService(_context);

            var o = new Observation()
            {
                On = "2001/01/01",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=Hello World" }
            };

            var o2 = new Observation()
            {
                On = "2001/01/02",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=More" }
            };

            var o3 = new Observation()
            {
                On = "2001/01/03",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=More More" }
            };

            service.RegisterObservation(o);
            service.RegisterObservation(o2);
            service.RegisterObservation(o3);

            var result = service.SearchState(new[] { "State.MyVal=Hello World" });

            Assert.Equal("E1", result.First().Entity);
            Assert.Equal(new DateTime(2001, 1, 1), result.First().Start);
            Assert.Equal(new DateTime(2001, 1, 2).AddSeconds(-1), result.First().End);

            Assert.Equal(1, result.Count);
        }

        [Fact]
        public void FilterByKey_LessThan_Returns_Only_Match_Results()
        {
            _context.Database.EnsureDeleted();
            var service = new TimeLineService(_context);

            var o = new Observation()
            {
                On = "2001/01/01",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=1" }
            };

            var o2 = new Observation()
            {
                On = "2001/01/02",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=10" }
            };


            service.RegisterObservation(o);
            service.RegisterObservation(o2);

            var result = service.SearchState(new[] { "State.MyVal < 10" });

            Assert.Equal("E1", result.First().Entity);
            Assert.Equal(new DateTime(2001, 1, 1), result.First().Start);
            Assert.Equal(new DateTime(2001, 1, 2).AddSeconds(-1), result.First().End);

            Assert.Equal(1, result.Count);
        }

        public void FilterByKey_LessThanEqual_Returns_Only_Match_Results()
        {
            _context.Database.EnsureDeleted();
            var service = new TimeLineService(_context);

            var o = new Observation()
            {
                On = "2001/01/01",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=10" }
            };

            var o2 = new Observation()
            {
                On = "2001/01/02",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=11" }
            };


            service.RegisterObservation(o);
            service.RegisterObservation(o2);

            var result = service.SearchState(new[] { "State.MyVal <= 10" });

            Assert.Equal("E1", result.First().Entity);
            Assert.Equal(new DateTime(2001, 1, 1), result.First().Start);
            Assert.Equal(new DateTime(2001, 1, 2).AddSeconds(-1), result.First().End);

            Assert.Equal(1, result.Count);
        }

        [Fact]
        public void FilterByKey_GreaterThan_Returns_Only_Match_Results()
        {
            _context.Database.EnsureDeleted();
            var service = new TimeLineService(_context);

            var o = new Observation()
            {
                On = "2001/01/01",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=1" }
            };

            var o2 = new Observation()
            {
                On = "2001/01/02",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=11" }
            };


            service.RegisterObservation(o);
            service.RegisterObservation(o2);

            var result = service.SearchState(new[] { "State.MyVal > 10" });

            Assert.Equal("E1", result.First().Entity);
            Assert.Equal(new DateTime(2001, 1, 2), result.First().Start);

            Assert.Equal(1, result.Count);
        }

        [Fact]
        public void FilterByKey_GreaterEqualThan_Returns_Only_Match_Results()
        {
            _context.Database.EnsureDeleted();
            var service = new TimeLineService(_context);

            var o = new Observation()
            {
                On = "2001/01/01",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=1" }
            };

            var o2 = new Observation()
            {
                On = "2001/01/02",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=10" }
            };


            service.RegisterObservation(o);
            service.RegisterObservation(o2);

            var result = service.SearchState(new[] { "State.MyVal >= 10" });

            Assert.Equal("E1", result.First().Entity);
            Assert.Equal(new DateTime(2001, 1, 2), result.First().Start);

            Assert.Equal(1, result.Count);
        }


        [Fact]
        public void FilterByAfter_AfterEndOfLine_ReturnsResults()
        {
            _context.Database.EnsureDeleted();
            var service = new TimeLineService(_context);

            var o = new Observation()
            {
                On = "2001/01/01",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=Hello World" }
            };

            var o2 = new Observation()
            {
                On = "2001/01/03",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=More" }
            };

            service.RegisterObservation(o);
            service.RegisterObservation(o2);

            var result = service.SearchState(new[] { "After=1/04/2001" }).First();

            Assert.Equal(new DateTime(2001, 1, 4), result.Start);
            Assert.Equal("More", result.Value);
            Assert.Null(result.End);
        }

        [Fact]
        public void FilterByAfter_BeforeStartOfLine_ReturnsResults()
        {
            _context.Database.EnsureDeleted();
            var service = new TimeLineService(_context);

            var o = new Observation()
            {
                On = "2001/01/01",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=Hello World" }
            };

            var o2 = new Observation()
            {
                On = "2001/01/03",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=More" }
            };

            service.RegisterObservation(o);
            service.RegisterObservation(o2);

            var result = service.SearchState(new[] { "After=1/01/2000" });

            Assert.Equal(2, result.Count());
            Assert.Equal(new DateTime(2001, 1, 1), result.First().Start);
            Assert.Equal(new DateTime(2001, 1, 3).AddSeconds(-1), result.First().End);
            Assert.Equal("Hello World", result.First().Value);


            Assert.Equal(new DateTime(2001, 1, 3), result.Skip(1).First().Start);
            Assert.Null(result.Skip(1).First().End);
            Assert.Equal("More", result.Skip(1).First().Value);
        }

        [Fact]
        public void FilterByAfter_Between_Observations_ReturnsResults()
        {
            _context.Database.EnsureDeleted();
            var service = new TimeLineService(_context);

            var o = new Observation()
            {
                On = "2001/01/01",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=Hello World" }
            };

            var o2 = new Observation()
            {
                On = "2001/01/03",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=More" }
            };

            service.RegisterObservation(o);
            service.RegisterObservation(o2);

            var result = service.SearchState(new[] { "After=1/02/2001" });

            Assert.Equal(2, result.Count());
            Assert.Equal(new DateTime(2001, 1, 2), result.First().Start);
            Assert.Equal(new DateTime(2001, 1, 3).AddSeconds(-1), result.First().End);
            Assert.Equal("Hello World", result.First().Value);


            Assert.Equal(new DateTime(2001, 1, 3), result.Skip(1).First().Start);
            Assert.Null(result.Skip(1).First().End);
            Assert.Equal("More", result.Skip(1).First().Value);
        }


        [Fact]
        public void FilterByBefore_ReturnsResults()
        {
            _context.Database.EnsureDeleted();
            var service = new TimeLineService(_context);

            var o = new Observation()
            {
                On = "2001/01/01",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=Hello World" }
            };

            var o2 = new Observation()
            {
                On = "2001/01/04",
                Entity = "E1",
                Expressions = new[] { "State.MyVal=More" }
            };

            service.RegisterObservation(o);
            service.RegisterObservation(o2);

            var result = service.SearchState(new[] { "Before=1/02/2001" });

            Assert.Equal(new DateTime(2001, 1, 1), result.First().Start);
            Assert.Equal(new DateTime(2001, 1, 2), result.First().End);
            Assert.Equal("Hello World", result.First().Value);
            Assert.Equal(1, result.Count);
        }


    }
}
