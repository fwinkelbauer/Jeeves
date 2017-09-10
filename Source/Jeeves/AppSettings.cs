using System.IO;
using Newtonsoft.Json;

namespace Jeeves
{
    internal class AppSettings
    {
        private const string SettingsPath = @"C:\ProgramData\Jeeves\settings.json";

        [JsonConstructor]
        public AppSettings(string baseUrl, bool useHttps, bool useAuthentication, string database, string sqlScriptsDirectory)
        {
            BaseUrl = baseUrl;
            UseHttps = useHttps;
            UseAuthentication = useAuthentication;
            Database = database;
            SqlScriptsDirectory = sqlScriptsDirectory;
        }

        public string BaseUrl { get; }

        public bool UseHttps { get; }

        public bool UseAuthentication { get; }

        public string Database { get; }

        public string SqlScriptsDirectory { get; }

        public static void WriteDefault()
        {
            Write(new AppSettings(
                "http://localhost:9042/jeeves/",
                false,
                true,
                @"C:\ProgramData\Jeeves\Jeeves.sqlite",
                @"C:\ProgramData\Jeeves\Scripts"));
        }

        public static void Write(AppSettings settings)
        {
            File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(settings));
        }

        public static AppSettings Load()
        {
            if (!File.Exists(SettingsPath))
            {
                WriteDefault();
            }

            return JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(SettingsPath));
        }
    }
}
