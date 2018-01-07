using Chronicity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Example.CryptoCurrency.Loaders
{
    public class CoinDesk
    {
        private ITimelineService _service;
        private string _apiKey;

        public CoinDesk(ITimelineService service)
        {
            _service = service;
            LoadBitCoin();
        }

        public void LoadBitCoin()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            var url = "https://api.coindesk.com";

            url = string.Format("{0}/v1/bpi/historical/close.json?start=2011-01-01&end={1}&currency=USD", url, DateTime.Today.ToString("yyyy-MM-dd"));

            var result = client.GetStringAsync(url).Result;

            var json = JObject.Parse(result);
            var raw = json["bpi"].ToObject<Dictionary<DateTime,double>>();

            var data = raw.Select(x => new Entry() { On = x.Key, Price = x.Value }).OrderBy(x => x.On);

            double lastPrice = 0;

            foreach (var price in data)
            {
                _service.RegisterEvent(new Chronicity.Core.Events.Event()
                {
                    Entity = "BTC",
                    On = price.On.ToShortDateString(),
                    Type = "Price",
                    Observations = new string[]
                                   {
                        string.Concat("Entity.State.Price=",price.Price),
                        string.Concat("Entity.State.Increase=", price.Price > lastPrice)
                                   }
                });

                lastPrice = price.Price;
            }

        }

        
        public class Entry
        {
            public DateTime On { get; set; }
            public Double Price { get; set; }
        }

    }
}
