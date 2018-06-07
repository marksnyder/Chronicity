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
                Expressions = new[] { "Entity.State.MyVal=Hello World" }
            };

            var o2 = new Observation()
            {
                On = "2001/01/02",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyVal=More" }
            };

            service.RegisterObservation(o);
            service.RegisterObservation(o2);

            var result = service.FilterState(new[] { "Entity.State.MyVal=Hello World" }).First();

            Assert.Equal("E1", result.Entity);
            Assert.Equal(new DateTime(2001, 1, 1), result.Start);
            Assert.Equal(new DateTime(2001, 1, 2), result.End);
        }
    }
}
