using Jeeves.Core;

namespace Jeeves
{
    internal sealed class Settings
    {
        public Settings(string databasePath, string baseUrl, SecurityOption security)
        {
            DatabasePath = databasePath;
            BaseUrl = baseUrl;
            Security = security;
        }

        public string DatabasePath { get; }
        public string BaseUrl { get; }
        public SecurityOption Security { get; }
    }
}
