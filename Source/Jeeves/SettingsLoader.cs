using System.IO;
using Jeeves.Core;
using Newtonsoft.Json;

namespace Jeeves
{
    internal static class SettingsLoader
    {
        public static Settings Load(string settingsPath)
        {
            if (!File.Exists(settingsPath))
            {
                WriteDefault(settingsPath);
            }

            return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsPath));
        }

        private static void WriteDefault(string settingsPath)
        {
            var settings = new Settings(
                "Jeeves",
                "A simple REST service which provides configuration data for applications",
                "Jeeves.sqlite",
                "http://localhost:9042/jeeves/",
                SecurityOption.Http);

            File.WriteAllText(settingsPath, JsonConvert.SerializeObject(settings, Formatting.Indented));
        }
    }
}
