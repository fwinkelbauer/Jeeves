using System.IO;
using Jeeves.Core;
using Newtonsoft.Json;

namespace Jeeves
{
    internal static class JeevesSettingsLoader
    {
        public static JeevesSettings Load(string settingsPath)
        {
            if (!File.Exists(settingsPath))
            {
                WriteDefault(settingsPath);
            }

            return JsonConvert.DeserializeObject<JeevesSettings>(File.ReadAllText(settingsPath));
        }

        private static void WriteDefault(string settingsPath)
        {
            var settings = new JeevesSettings(
                "http://localhost:9042/jeeves/",
                SecurityOption.Http);

            File.WriteAllText(settingsPath, JsonConvert.SerializeObject(settings, Formatting.Indented));
        }
    }
}
