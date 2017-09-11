using System;

namespace Jeeves.Core
{
    public class JeevesSettings
    {
        public JeevesSettings(string baseUrl, bool useHttps, bool useAuthentication)
        {
            BaseUrl = baseUrl.ThrowIfNull(nameof(baseUrl));
            UseHttps = useHttps;
            UseAuthentication = useAuthentication;
        }

        public string BaseUrl { get; }

        public bool UseHttps { get; }

        public bool UseAuthentication { get; }
    }
}
