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
    public class GuardianNews
    {
        private ITimelineService _service;
        private string _apiKey;

        public GuardianNews(ITimelineService service, string apiKey)
        {
            _service = service;
            _apiKey = apiKey;
            LoadNews(1);
        }

        public void LoadNews(int page)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            var url = "https://content.guardianapis.com/";

            url = string.Format("{0}search?api-key={1}&q=bitcoin&from-date=2012-01-01&page-size=50&page={2}", url, _apiKey, page);

            var result = client.GetStringAsync(url).Result;

            var json = JObject.Parse(result);
            var data = json["response"].ToObject<Response>();

            foreach(var headline in data.results)
            {
                _service.RegisterEvent(new Chronicity.Core.Events.Event()
                {
                    Entity = "Guardian",
                    On = headline.webPublicationDate,
                    Type = "Headline",
                    Observations = new string[]
                 {
                        string.Concat("Entity.State.Title=",headline.webTitle),
                        string.Concat("Entity.State.WebLink=",headline.webUrl),
                        "Entity.Links.Add=BTC"
                 }
                });
            }

            if (data.pages > 1 && page < data.pages)
            {
                LoadNews(page + 1);
            }
        }

        public class Result
        {
            public string webTitle { get; set; }
            public string webPublicationDate { get; set; }
            public string webUrl { get; set; }
        }

        public class Response
        {
            public int pages { get; set; }
            public List<Result> results { get; set; }
        }
    }
}
