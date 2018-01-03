using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Chronicity.Core;

namespace Example.CatsAndDogs.DataLoad
{
    public class LoadIntake
    {
        private ITimelineService _service;
        private string _fileName;

        public LoadIntake(string filename, ITimelineService service)
        {
            _service = service;
            _fileName = filename;
        }

        public void Run()
        {
            var data = System.IO.File.ReadAllLines(_fileName);

            int counter = 0;

            foreach(var line in data.Skip(1))
            {
                var start = DateTime.Now;

                var fields = line.Split(',');
                var id = fields[0];
                var name = fields[1];
                var date = fields[2];
                var animalType = fields[7];
                var lastFound = fields[4];
                var lastIntake = fields[5];
                var lastIntakeCondition = fields[6];
                var breed = fields[10];
                var color = fields[11];

                counter++;

                _service.RegisterEvent(new Chronicity.Core.Events.Event()
                {
                     Entity = id,
                     On = date,
                     Type = "Intake",
                     Observations = new string[] {
                         String.Format("Entity.State.AnimalType={0}",animalType),
                         String.Format("Entity.State.Name={0}",name),
                         String.Format("Entity.State.FoundAt={0}",lastFound),
                         String.Format("Entity.State.Intake={0}",lastIntake),
                         String.Format("Entity.State.Condition={0}",lastIntakeCondition),
                         String.Format("Entity.State.Breed={0}",breed),
                         String.Format("Entity.State.Color={0}",color),
                         String.Format("Entity.Links.Add=Weather")
                     }
                });


                if(counter % 100 == 0)
                {
                    Trace.WriteLine(string.Concat(DateTime.Now.Subtract(start).TotalMilliseconds, "  -  ", counter));
                }
                
            }
        }
    }
}
