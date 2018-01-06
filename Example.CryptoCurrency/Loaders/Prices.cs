using Chronicity.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Example.CryptoCurrency.Loaders
{
    public class Prices
    {
        private ITimelineService _service;

        public Prices(ITimelineService service)
        {
            _service = service;
            LoadBitcoin();
        }

        private void LoadBitcoin()
        {
            var lines = System.IO.File.ReadAllLines( System.IO.Path.Combine("./SampleData/prices/bitcoin_price.csv"));

            foreach(var line in lines.Skip(1).Reverse())
            {
                var fields = line.Replace("\"","").Split(',');

                var md = fields[0].Split(' ');
                var datetime = string.Format("{0}-{1}-{2}", md[1], md[0], fields[1].Trim());
                var open = Double.Parse(fields[2]);
                var close = Double.Parse(fields[5]);


                _service.RegisterEvent(new Chronicity.Core.Events.Event()
                {
                    Entity = "BTC",
                    On = DateTime.ParseExact(datetime, "d-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None).ToShortDateString(),
                    Type = "Price",
                    Observations = new string[]
                    {
                        string.Concat("Entity.State.Open=",fields[2]),
                        string.Concat("Entity.State.High=",fields[3]),
                        string.Concat("Entity.State.Low=",fields[4]),
                        string.Concat("Entity.State.Close=",fields[5]),
                        string.Concat("Entity.State.Increase=", close > open)
                    }
                });
            }
        }
    }
}
