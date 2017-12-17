using Chronicity.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Chronicity.Core.Events;
using System.Linq;

namespace Provider.InMemory.Tests.SampleTimelines
{
    public class WhoDoneIt
    {
        public static void Build(ITimelineService service)
        {
            service.RegisterEntity("The Butler", "suspect");
            service.RegisterEntity("The Maid", "suspect");
            service.RegisterEntity("The Cook", "suspect");

            service.RegisterEntity("Bedroom", "room");
            service.RegisterEntity("Living Room", "room");
            service.RegisterEntity("Kitchen", "room");



            var events = new List<Event>()
            {
                // Init the state of the rooms

                new Event() {  Entity = "Bedroom", On = "1/1/1984 01:00", Type = "Built",
                Observations = new string[] {
                    "State.DistanceFromWashroom=10"
                } },

                new Event() {  Entity = "Living Room", On = "1/1/1984 01:00", Type = "Built",
                Observations = new string[] {
                    "State.DistanceFromWashroom=20"
                } },

                new Event() {  Entity = "Kitchen", On = "1/1/1984 01:00", Type = "Built",
                Observations = new string[] {
                    "State.DistanceFromWashroom=30"
                } },


                // Everyone starts in livingroom with a knife
                new Event() {  Entity = "The Butler", On = "1/1/2001 05:00", Type = "Seen",
                Observations = new string[] {
                    "State.HasKnife=false",
                    "Link.Room=Living Room"
                } },

                new Event() {  Entity = "The Cook", On = "1/1/2001 05:00", Type = "Seen",
                Observations = new string[] {
                    "State.HasKnife=true",
                    "Link.Room=Living Room"
                } },

                new Event() {  Entity = "The Maid", On = "1/1/2001 05:00", Type = "Seen",
                Observations = new string[] {
                    "State.HasKnife=false",
                    "Link.Room=Living Room"
                } },


                // Move 1
                new Event() {  Entity = "The Butler", On = "1/1/2001 05:10", Type = "Seen",
                Observations = new string[] {
                    "State.HasKnife=true",
                    "Link.Room=Kitchen"
                } },

                new Event() {  Entity = "The Cook", On = "1/1/2001 05:10", Type = "Seen",
                Observations = new string[] {
                    "State.HasKnife=true",
                    "Link.Room=Kitchen"
                } },

                new Event() {  Entity = "The Maid", On = "1/1/2001 05:10", Type = "Seen",
                Observations = new string[] {
                    "State.HasKnife=true",
                    "Link.Room=Bedroom"
                } },


                // Move 2
                new Event() {  Entity = "The Butler", On = "1/1/2001 05:20", Type = "Seen",
                Observations = new string[] {
                    "State.HasKnife=true",
                    "Link.Room=Living Room"
                } },

                new Event() {  Entity = "The Cook", On = "1/1/2001 05:20", Type = "Seen",
                Observations = new string[] {
                    "State.HasKnife=false",
                    "Link.Room=Bedroom"
                } },

                new Event() {  Entity = "The Maid", On = "1/1/2001 05:20", Type = "Seen",
                Observations = new string[] {
                    "State.HasKnife=false",
                    "Link.Room=Bedroom"
                } },

            };

            events.ForEach(x => service.RegisterEvent(x));

        }
    }
}
