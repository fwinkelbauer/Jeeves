namespace Jeeves
{
    internal sealed class Settings
    {
        public Settings(string baseUrl, string databaseFile, string logDirectory)
        {
            BaseUrl = baseUrl;
            DatabaseFile = databaseFile;
            LogDirectory = logDirectory;
        }

        public string BaseUrl { get; }

        public string DatabaseFile { get; }

        public string LogDirectory { get; }
    }
}
