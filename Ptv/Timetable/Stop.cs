using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ptv.Timetable
{
    [JsonObject()]
    public class Stop : Location
    {
        [JsonProperty(PropertyName = "transport_type")]
        public TransportType TransportType { get; set; }

        [JsonProperty(PropertyName = "stop_id")]
        public string StopID { get; set; }
    }
}
