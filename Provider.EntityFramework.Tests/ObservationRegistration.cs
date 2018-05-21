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
    public class ObservationRegistration
    {
        private TimeLineService _service;

        public ObservationRegistration()
        {
            var options = new DbContextOptionsBuilder<ChronicityContext>()
                .UseInMemoryDatabase(databaseName: "ObservationRegistration")
                .Options;

            _service = new TimeLineService(new ChronicityContext(options));
        }



        [Fact]
        public void RegisteringObservation_StateIsSet()
        {
            var o = new Observation()
            {
                On = "2001/01/01",
                Type = "MyEventType",
                Entity = "E1" ,
                Expressions = new[] { "Entity.State.MyVal=Hello World" }
            };

            _service.RegisterObservation(o);

            Assert.Equal("Hello World", _service.GetEntityState("E1", "2001/01/01")["MyVal"]);
        }

        [Fact]
        public void RegisteringObservation_StateIsMerged()
        {

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

            _service.RegisterObservation(o1);
            _service.RegisterObservation(o2);

            Assert.Equal("Hello World", _service.GetEntityState("E1", "2001/01/01 01:02")["MyVal"]);
            Assert.Equal("Hello World Again", _service.GetEntityState("E1", "2001/01/01 01:02")["MyNextVal"]);
        }

        [Fact]
        public void RegisteringObservation_StateIsMerged_Chronologically()
        {

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

            _service.RegisterObservation(o1);
            _service.RegisterObservation(o2);

            Assert.False(_service.GetEntityState("E1", "2001/01/01 01:01").ContainsKey("MyNextVal"));
        }




        [Fact]
        public void RegisteringObservation_StateIsMerged_Chronologically2()
        {

            var o1 = new Observation()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entity = "E1"
            };

            var o2 = new Observation()
            {
                On = "2001/01/01 01:03",
                Type = "MyEventType",
                Entity = "E1"
            };

            var o3 = new Observation()
            {
                On = "2001/01/01 01:02",
                Type = "MyEventType",
                Entity = "E1"
            };


            _service.RegisterObservation(o1);
            _service.RegisterObservation(o2);
            _service.RegisterObservation(o3);

            Assert.False(_service.GetEntityState("E1", "2001/01/01 01:03").ContainsKey("MyVal1"));
            Assert.False(_service.GetEntityState("E1", "2001/01/01 01:03").ContainsKey("MyVal2"));
            Assert.False(_service.GetEntityState("E1", "2001/01/01 01:03").ContainsKey("MyVal3"));
        }

        [Fact]
        public void RegisteringObservation_StateIsMerged_Simultaneously()
        {

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


            _service.RegisterObservation(o1);
            _service.RegisterObservation(o2);

            Assert.Equal("Hello World", _service.GetEntityState("E1", "2001/01/01 01:01")["MyVal"]);
            Assert.Equal("Hello World Again", _service.GetEntityState("E1", "2001/01/01 01:01")["MyNextVal"]);
        }

        [Fact]
        public void RegisteringObservation_StateIsMerged_MultipleExpressions()
        {

            var o1 = new Observation()
            {
                On = "2001/01/01 01:01",
                Type = "MyEventType",
                Entity = "E1",
                Expressions = new[] { "Entity.State.MyVal=Hello World", "Entity.State.MyNextVal=Hello World Again" }
            };

            _service.RegisterObservation(o1);

            Assert.Equal("Hello World", _service.GetEntityState("E1", "2001/01/01 01:01")["MyVal"]);
            Assert.Equal("Hello World Again", _service.GetEntityState("E1", "2001/01/01 01:01")["MyNextVal"]);
        }

        [Fact]
        public void RegisteringObservation_StateIsOverwritten()
        {

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


            _service.RegisterObservation(o1);
            _service.RegisterObservation(o2);

            var contexts = _service.FilterEvents(new string[] { }).OrderByDescending(x => x.Event.On);

            Assert.Equal("Hello World Again", _service.GetEntityState("E1", "2001/01/01 01:02")["MyNextVal"]);
        }
    }
}
