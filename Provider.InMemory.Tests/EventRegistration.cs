using Chronicity.Provider.InMemory;
using System;
using Xunit;
using Chronicity.Core.Events;
using System.Collections.Generic;
using System.Linq;

namespace Provider.InMemory.Tests
{
    public class EventRegistration
    {
        [Fact]
        public void RegisteringEvent_Must_Be_RegisteredEntity()
        {
            var service = new TimeLineService();
            var e = new Event()
            {
                 On = "2001/01/01",
                 Type = "MyEventType"
            };

            Assert.Throws<Exception>(() => service.RegisterEvent(e));
        }


        [Fact]
        public void RegisteringEvent_CanBeRetrieved()
        {
            var service = new TimeLineService();
            var e = new Event()
            {
                On = "2001/01/01",
                Type = "MyEventType",
                Entity = "E1"
            };

            service.RegisterEntity("E1", "MyEntityType");
            service.RegisterEvent(e);

            var contexts = service.FilterEvents(new string[] {});
            
            Assert.Single(contexts);

        }

        [Fact]
        public void RegisteringEvent_StateIsSet()
        {
            var service = new TimeLineService();
            var e = new Event()
            {
                On = "2001/01/01",
                Type = "MyEventType",
                Entity = "E1",
                Observations = new string[] { "State.MyVal=Hello World" }
            };

            service.RegisterEntity("E1", "MyEntityType");
            service.RegisterEvent(e);

            var contexts = service.FilterEvents(new string[] { });

            Assert.Equal("Hello World", contexts.First().State["MyVal"]);
        }

        [Fact]
        public void RegisteringEvent_StateIsMerged()
        {
            var service = new TimeLineService();

            var e1 = new Event()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entity = "E1",
                Observations = new string[] { "State.MyVal=Hello World" }
            };

            var e2 = new Event()
            {
                On = "2001/01/01 01:02",
                Type = "MyEventType",
                Entity = "E1",
                Observations = new string[] { "State.MyNextVal=Hello World Again" }
            };

            service.RegisterEntity("E1", "MyEntityType");

            service.RegisterEvent(e1);
            service.RegisterEvent(e2);

            var context = service.FilterEvents(new string[] { })
                .OrderByDescending(x => x.Event.On)
                .First();

            Assert.Equal("Hello World", context.State["MyVal"]);
            Assert.Equal("Hello World Again", context.State["MyNextVal"]);
        }


        [Fact]
        public void RegisteringEvent_StateIsMerged_Chronologically()
        {
            var service = new TimeLineService();

            var e1 = new Event()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entity = "E1",
                Observations = new string[] { "State.MyVal=Hello World" }
            };

            var e2 = new Event()
            {
                On = "2001/01/01 01:02",
                Type = "MyEventType",
                Entity = "E1",
                Observations = new string[] { "State.MyNextVal=Hello World Again" }
            };

            service.RegisterEntity("E1", "MyEntityType");

            service.RegisterEvent(e1);
            service.RegisterEvent(e2);

            var contexts = service.FilterEvents(new string[] { }).OrderBy(x => x.Event.On);

            Assert.False(contexts.First().State.ContainsKey("MyNextVal"));
        }

        [Fact]
        public void RegisteringEvent_StateIsMerged_Simultaneously()
        {
            var service = new TimeLineService();

            var e1 = new Event()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entity = "E1",
                Observations = new string[] { "State.MyVal=Hello World" }
            };

            var e2 = new Event()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entity = "E1",
                Observations = new string[] { "State.MyNextVal=Hello World Again" }
            };

            service.RegisterEntity("E1", "MyEntityType");

            service.RegisterEvent(e1);
            service.RegisterEvent(e2);

            var contexts = service.FilterEvents(new string[] { }).OrderBy(x => x.Event.On);

            Assert.True(contexts.First().State.ContainsKey("MyNextVal"));
            Assert.True(contexts.First().State.ContainsKey("MyVal"));
            Assert.True(contexts.Skip(1).First().State.ContainsKey("MyNextVal"));
            Assert.True(contexts.Skip(1).First().State.ContainsKey("MyVal"));
        }


        [Fact]
        public void RegisteringEvent_StateIsOverwritten()
        {
            var service = new TimeLineService();

            var e1 = new Event()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entity = "E1",
                Observations = new string[] { "State.MyVal=Hello World" }
            };

            var e2 = new Event()
            {
                On = "2001/01/01 01:02",
                Type = "MyEventType",
                Entity = "E1",
                Observations = new string[] { "State.MyVal=Hello World Again" }
            };

            service.RegisterEntity("E1", "MyEntityType");

            service.RegisterEvent(e1);
            service.RegisterEvent(e2);

            var contexts = service.FilterEvents(new string[] { }).OrderByDescending(x => x.Event.On);

            Assert.Equal("Hello World Again", contexts.First().State["MyVal"]);
        }

    }
}
