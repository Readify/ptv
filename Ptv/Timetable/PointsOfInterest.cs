using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ptv.Timetable
{
    [JsonObject()]
    public class PointsOfInterest
    {
        [JsonProperty(PropertyName = "minLat")]
        public decimal MinimumLatitude { get; set; }

        [JsonProperty(PropertyName = "minLong")]
        public decimal MinimumLongitude { get; set; }

        [JsonProperty(PropertyName = "maxLat")]
        public decimal MaximumLatitude { get; set; }

        [JsonProperty(PropertyName = "maxLong")]
        public decimal MaximumLongitude { get; set; }

        [JsonProperty(PropertyName = "weightedLat")]
        public decimal WeightedLatitude { get; set; }

        [JsonProperty(PropertyName = "weightedLong")]
        public decimal WeightedLongitude { get; set; }

        [JsonProperty(PropertyName = "totalLocations")]
        public uint TotalLocations { get; set; }
    }
}
