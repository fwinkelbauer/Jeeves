using Jeeves.Core;

namespace Jeeves
{
    internal sealed class Settings
    {
        public Settings(string serviceName, string serviceDescription, string databasePath, string baseUrl, SecurityOption security)
        {
            ServiceName = serviceName;
            ServiceDescription = serviceDescription;
            DatabasePath = databasePath;
            BaseUrl = baseUrl;
            Security = security;
        }

        public string ServiceName { get; }
        public string ServiceDescription { get; }
        public string DatabasePath { get; }
        public string BaseUrl { get; }
        public SecurityOption Security { get; }
    }
}
