#if !ELYTOPIA_DISABLE_SERIALAZER
using Newtonsoft.Json;

namespace Elytopia.Serialization
{
    internal static class JsonProjectSettings
    {
        public static void ApplySettings()
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new JsonAnalyticsDataConverter());

            JsonConvert.DefaultSettings = () => settings;
        }
    }
}
#endif