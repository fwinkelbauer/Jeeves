using System.IO;
using Newtonsoft.Json;

namespace Jeeves
{
    internal class AppSettings
    {
        [JsonConstructor]
        private AppSettings(string baseUrl, bool useHttps, bool useAuthentication, string database, string sqlScriptsDirectory)
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

        public static void CreateDefault(string path)
        {
            var settings = new AppSettings(
                    "http://localhost:9042/jeeves/",
                    false,
                    true,
                    @"C:\ProgramData\Jeeves\Jeeves.sqlite",
                    @"C:\ProgramData\Jeeves\Scripts");

            File.WriteAllText(path, JsonConvert.SerializeObject(settings));
        }

        public static AppSettings Load(string path)
        {
            return JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(path));
        }
    }
}
