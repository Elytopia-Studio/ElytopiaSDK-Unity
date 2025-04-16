using System;
using Elytopia.Analytics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Elytopia.Serialization
{
    public class JsonAnalyticsDataConverter : JsonConverter<AnalyticsEventData>
    {
        protected JsonSerializer InternalSerializer = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };
        
        public override void WriteJson(JsonWriter writer, AnalyticsEventData value, JsonSerializer serializer)
        {
            InternalSerializer.Serialize(writer, value);
        }

        public override AnalyticsEventData ReadJson(JsonReader reader, Type objectType, AnalyticsEventData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var type = jsonObject["Type"].ToObject<AnalyticsDataType>();

            return type switch
            {
                AnalyticsDataType.Undefined => jsonObject.ToObject<AnalyticsEventData>(InternalSerializer),
                AnalyticsDataType.FpsEvent => jsonObject.ToObject<AnalyticsEventData>(InternalSerializer),
                _ => GetExtendedAnalyticsData(type, jsonObject)
            };
        }

        public virtual AnalyticsEventData GetExtendedAnalyticsData(AnalyticsDataType type, JObject jsonObject)
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}