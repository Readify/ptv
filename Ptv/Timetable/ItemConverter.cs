using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ptv.Timetable
{
    public class ItemConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(Item))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var itemWrapper = JObject.Load(reader);
            var type = itemWrapper["type"].Value<string>();
            var result = itemWrapper["result"];

            switch (type)
            {
                case "stop":
                    var stop = result.ToObject<Stop>();
                    return stop;
                    break;

                case "line":
                    var line = result.ToObject<Line>();
                    return line;
                    break;

                default:
                    throw new TimetableException();
                    break;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
