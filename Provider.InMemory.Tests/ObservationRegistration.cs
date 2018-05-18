using Chronicity.Provider.InMemory;
using System;
using Xunit;
using Chronicity.Core.Events;
using System.Collections.Generic;
using System.Linq;


namespace Provider.InMemory.Tests
{
    public class ObservationRegistration
    {
        [Fact]
        public void RegisteringObservation_StateIsSet()
        {
            var service = new TimeLineService();
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
            var service = new TimeLineService();

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
            var service = new TimeLineService();

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



        //TODO
        //[Fact]
        //public void RegisteringObservation_StateIsMerged_Chronologically2()
        //{
        //    var service = new TimeLineService();

        //    var e1 = new Event()
        //    {
        //        On = "2001/01/01 01:01",
        //        Type = "MyEventType",
        //        Entity = "E1",
        //        Observations = new string[] { "Entity.State.MyVal1=Hello World" }
        //    };

        //    var e2 = new Event()
        //    {
        //        On = "2001/01/01 01:03",
        //        Type = "MyEventType",
        //        Entity = "E1",
        //        Observations = new string[] { "Entity.State.MyVal2=Hello World Again" }
        //    };

        //    var e3 = new Event()
        //    {
        //        On = "2001/01/01 01:02",
        //        Type = "MyEventType",
        //        Entity = "E1",
        //        Observations = new string[] { "Entity.State.MyVal3=Hello World Again" }
        //    };


        //    service.RegisterEvent(e1);
        //    service.RegisterEvent(e2);
        //    service.RegisterEvent(e3);

        //    var contexts = service.FilterEvents(new string[] { }).OrderBy(x => x.Event.On);

        //    Assert.False(contexts.Last().State.ContainsKey("MyVal1"));
        //    Assert.False(contexts.Last().State.ContainsKey("MyVal2"));
        //    Assert.False(contexts.Last().State.ContainsKey("MyVal3"));
        //}

        [Fact]
        public void RegisteringObservation_StateIsMerged_Simultaneously()
        {
            var service = new TimeLineService();

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
        public void RegisteringObservation_StateIsOverwritten()
        {
            var service = new TimeLineService();

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

            var contexts = service.FilterEvents(new string[] { }).OrderByDescending(x => x.Event.On);

            Assert.Equal("Hello World Again", service.GetEntityState("E1", "2001/01/01 01:02")["MyNextVal"]);
        }
    }
}
