namespace Jeeves
{
    internal sealed class Settings
    {
        public Settings(string databasePath, string baseUrl)
        {
            DatabasePath = databasePath;
            BaseUrl = baseUrl;
        }

        public string DatabasePath { get; }
        public string BaseUrl { get; }
    }
}
