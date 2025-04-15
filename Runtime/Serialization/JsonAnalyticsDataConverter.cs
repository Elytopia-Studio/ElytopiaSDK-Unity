using System;
using Elytopia.Analytics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Elytopia.Serialization
{
    public class JsonAnalyticsDataConverter : JsonConverter<AnalyticsEventData>
    {
        private static readonly JsonSerializer _internalSerializer = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };
        
        public override void WriteJson(JsonWriter writer, AnalyticsEventData value, JsonSerializer serializer)
        {
            _internalSerializer.Serialize(writer, value);
        }

        public override AnalyticsEventData ReadJson(JsonReader reader, Type objectType, AnalyticsEventData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var type = jsonObject["Type"].ToObject<AnalyticsDataType>();

            return type switch
            {
                AnalyticsDataType.Undefined => jsonObject.ToObject<AnalyticsEventData>(),
                AnalyticsDataType.FpsEvent => jsonObject.ToObject<AnalyticsEventData>(),
                _ => jsonObject.ToObject<AnalyticsEventData>()
            };
        }
    }
}