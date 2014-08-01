using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ptv.Timetable
{
    [JsonObject()]
    public class SearchResult
    {
        [JsonProperty(PropertyName = "result")]
        public SearchResultItem Item { get; set; }

        [JsonProperty(PropertyName = "type")]
        public SearchResultType Stop { get; set; }
    }
}
