using Chronicity.Core;
using Chronicity.Core.Agent;
using Chronicity.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleCommon
{
    public class BirdDepartureAgent : IStateChangeReaction
    {
        public Reaction OnChange(string entity, string key, string priorValue, string newValue, string on)
        {
            var result = new Reaction(); 

            if (key == "proxa" && priorValue != string.Empty)
            {
                if(Convert.ToInt32(newValue) >= 30 && Convert.ToInt32(priorValue) < 30)
                {
                    result.NewEvents = new List<NewEvent>() { new NewEvent() {
                         Entities = new string [] { entity },
                         On = on,
                         Type = "Bird Departed"
                    } };

                    result.NewObservations = new List<Observation>() {
                        new Observation() {
                             Entity = entity,
                             On = on,
                            Expressions = new string [] {"Entity.State.activeBird=False" }
                        } };
                }
            }

            return result;
        }
    }
}
