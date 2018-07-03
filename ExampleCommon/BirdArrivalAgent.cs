﻿using Chronicity.Core;
using Chronicity.Core.Agent;
using Chronicity.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleCommon
{
    public class BirdArrivalAgent : IStateChangeReaction
    {
        public Reaction OnChange(string entity, string key, string priorValue, string newValue, string on)
        {
            var result = new Reaction(); 

            if (key == "proxa" && priorValue != string.Empty)
            {
                if(Convert.ToInt32(newValue) < 7 && Convert.ToInt32(priorValue) >= 7)
                {
                    result.NewEvents = new List<NewEvent>() { new NewEvent() {
                         Entities = new string [] { entity },
                         On = on,
                         Type = "Bird Arrived"
                    } };

                    result.NewObservations = new List<Observation>() {
                        new Observation() {
                             Entity = entity,
                             On = on,
                            Expressions = new string[] {"Entity.State.activeBird=True" }
                        } };
                }
            }

            return result;
        }
    }
}