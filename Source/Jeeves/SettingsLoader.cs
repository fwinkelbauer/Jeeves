using System;
using System.IO;
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

            try
            {
                var settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsPath));

                if (settings == null)
                {
                    throw new InvalidOperationException($"Empty configuration - '{settingsPath}'");
                }

                return settings;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Invalid configuration - '{settingsPath}'", e);
            }
        }

        private static void WriteDefault(string settingsPath)
        {
            var settings = new Settings(
                "Jeeves.sqlite",
                "http://localhost:9042/jeeves/");

            File.WriteAllText(settingsPath, JsonConvert.SerializeObject(settings, Formatting.Indented));
        }
    }
}
