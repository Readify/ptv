using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ptv.Timetable
{
    [JsonObject()]
    public class SearchResultItem
    {
        [JsonProperty(PropertyName = "suburb")]
        public string Suburb { get; set; }

        [JsonProperty(PropertyName = "transport_type")]
        public TransportType TransportType { get; set; }

        [JsonProperty(PropertyName = "stop_id")]
        public string StopID { get; set; }

        [JsonProperty(PropertyName = "location_name")]
        public string LocationName { get; set; }

        [JsonProperty(PropertyName = "lat")]
        public decimal Latitude { get; set; }

        [JsonProperty(PropertyName = "long")]
        public decimal Longitude { get; set; }

        [JsonProperty(PropertyName = "distance")]
        public decimal Distance { get; set; }
    }
}
