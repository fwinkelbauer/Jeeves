namespace Jeeves.Core
{
    public class JeevesSettings
    {
        public JeevesSettings(string baseUrl, SecurityOption security)
        {
            BaseUrl = baseUrl.ThrowIfNull(nameof(baseUrl));
            Security = security;
        }

        public string BaseUrl { get; }

        public SecurityOption Security { get; }
    }
}
