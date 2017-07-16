namespace Jeeves.Core
{
    public class JeevesSettings
    {
        public JeevesSettings(bool useHttps, bool useAuthentication)
        {
            UseHttps = useHttps;
            UseAuthentication = useAuthentication;
        }

        public bool UseHttps { get; }

        public bool UseAuthentication { get; }
    }
}
