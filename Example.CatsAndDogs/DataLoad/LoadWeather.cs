using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Chronicity.Core;

namespace Example.CatsAndDogs.DataLoad
{
    public class LoadWeather
    {
        private ITimelineService _service;
        private string _fileName;

        public LoadWeather(string filename, ITimelineService service)
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
                var date = fields[5];
                var temp = fields[13];

                counter++;

                _service.RegisterEvent(new Chronicity.Core.Events.Event()
                {
                     Entity = "Weather",
                     On = date,
                     Type = "Reading",
                     Observations = new string[] {
                         String.Format("Entity.State.Temp={0}",temp)
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
