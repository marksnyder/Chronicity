using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBrowser.Models
{
    public class DetailView
    {
        public string On { get; set; }
        public string Id { get; set; }
        public List<StateValue> StateValues { get; set; }
    }
}
