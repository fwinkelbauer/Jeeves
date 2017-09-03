using System;
using System.IO;

namespace Jeeves
{
    internal class AppSettings
    {
        private AppSettings()
        {
        }

        public Uri BaseUrl { get; private set; }

        public bool UseHttps { get; private set; }

        public bool UseAuthentication { get; private set; }

        public FileInfo Database { get; private set; }

        public string SqlScriptsDirectory { get; private set; }

        public static AppSettings Load()
        {
            var settings = new AppSettings()
            {
                BaseUrl = new Uri(TryGetString("JEEVES_BASE_URL", "http://localhost:9042/jeeves/")),
                UseHttps = TryGetBool("JEEVES_USE_HTTPS", false),
                UseAuthentication = TryGetBool("JEEVES_USE_AUTHENTICATION", true),
                Database = new FileInfo(TryGetString("JEEVES_DATABASE", @"C:\ProgramData\Jeeves\Jeeves.sqlite")),
                SqlScriptsDirectory = TryGetString("JEEVES_SQL_SCRIPTS_DIRECTORY", @"C:\ProgramData\Jeeves\Scripts")
            };

            return settings;
        }

        private static bool TryGetBool(string variable, bool alternative)
        {
            return TryGetValue(variable, alternative, Convert.ToBoolean);
        }

        private static string TryGetString(string variable, string alternative)
        {
            return TryGetValue(variable, alternative, v => v);
        }

        private static T TryGetValue<T>(string variable, T alternative, Func<string, T> converter)
        {
            var value = Environment.GetEnvironmentVariable(variable);

            if (value == null)
            {
                return alternative;
            }

            try
            {
                return converter(value);
            }
            catch (Exception)
            {
                throw new ArgumentException($"Could not parse value '{value}' to type {typeof(T)}");
            }
        }
    }
}
