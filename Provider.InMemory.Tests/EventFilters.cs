using Chronicity.Provider.InMemory;
using System;
using Xunit;
using Chronicity.Core.Events;
using System.Collections.Generic;
using System.Linq;

namespace Provider.InMemory.Tests
{
    public class EventFilters
    {

        [Fact]
        public void FilterEvent_BasicStateMatch()
        {
            var service = new TimeLineService();

            var e1 = new Event()
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
                Expressions = new string[] { "Entity.State.MyVal=Hello World" }
            };

            service.RegisterObservation(o1);
            service.RegisterEvent(e1);


            var match = service.FilterEvents(new string[] { "Entity.State.MyVal=Hello World" });
            var nonMatch = service.FilterEvents(new string[] { "Entity.State.MyVal=Not The One!" });

            Assert.Single(match);
            Assert.Empty(nonMatch);
        }


        [Fact]
        public void FilterEvent_BasicStateMatch_OutOfOrder()
        {
            var service = new TimeLineService();

            var e1 = new Event()
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
                Expressions = new string[] { "Entity.State.MyVal=Hello World" }
            };

            service.RegisterEvent(e1);
            service.RegisterObservation(o1);


            var match = service.FilterEvents(new string[] { "Entity.State.MyVal=Hello World" });
            var nonMatch = service.FilterEvents(new string[] { "Entity.State.MyVal=Not The One!" });

            Assert.Single(match);
            Assert.Empty(nonMatch);
        }


        [Fact]
        public void FilterEvent_TimeMatch_After()
        {
            var service = new TimeLineService();

            var e1 = new Event()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType1",
                Entities = new [] { "E1" }
            };

            var e2 = new Event()
            {
                On = "2001/01/01 01:02",
                Type = "MyEventType2",
                Entities = new [] { "E1" }
            };

            service.RegisterEvent(e1);
            service.RegisterEvent(e2);

            var match = service.FilterEvents(new string[] { "On.After=2001/01/01 01:01" });


            Assert.Single(match);
            Assert.Equal("MyEventType2", match.First().Event.Type);
        }


        [Fact]
        public void FilterEvent_TimeMatch_Before()
        {
            var service = new TimeLineService();

            var e1 = new Event()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType1",
                Entities = new [] { "E1" }
            };

            var e2 = new Event()
            {
                On = "2001/01/01 01:02",
                Type = "MyEventType2",
                Entities = new [] { "E1" }
            };

            service.RegisterEvent(e1);
            service.RegisterEvent(e2);

            var match = service.FilterEvents(new string[] { "On.Before=2001/01/01 01:02" });


            Assert.Single(match);
            Assert.Equal("MyEventType1", match.First().Event.Type);
        }


        [Fact]
        public void FilterEvent_TimeMatch_Between()
        {
            var service = new TimeLineService();

            var e1 = new Event()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType1",
                Entities = new [] { "E1" }
            };

            var e2 = new Event()
            {
                On = "2001/01/01 01:03",
                Type = "MyEventType2",
                Entities = new [] { "E1" }
            };

            service.RegisterEvent(e1);
            service.RegisterEvent(e2);

            var match = service.FilterEvents(new string[] { "On.Between=2001/01/01 01:00,2001/01/01 01:02" });

            Assert.Single(match);
            Assert.Equal("MyEventType1", match.First().Event.Type);
        }



        [Fact]
        public void FilterEvent_EventTypeMatch()
        {
            var service = new TimeLineService();

            var e1 = new Event()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entities = new[] { "E1" }
            };

            service.RegisterEvent(e1);

            var match = service.FilterEvents(new string[] { "Type=MyEventType" });
            var nonMatch = service.FilterEvents(new string[] { "Type=Not The One!" });

            Assert.Single(match);
            Assert.Empty(nonMatch);
        }


    }
}
