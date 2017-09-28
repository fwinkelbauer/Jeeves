using System;
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

            Settings settings = null;

            try
            {
                settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsPath));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Invalid configuration - '{settingsPath}'", e);
            }

            if (settings == null)
            {
                throw new InvalidOperationException($"Empty configuration - '{settingsPath}'");
            }

            return settings;
        }

        private static void WriteDefault(string settingsPath)
        {
            var settings = new Settings(
                "Jeeves.sqlite",
                "http://localhost:9042/jeeves/",
                SecurityOption.Http);

            File.WriteAllText(settingsPath, JsonConvert.SerializeObject(settings, Formatting.Indented));
        }
    }
}
