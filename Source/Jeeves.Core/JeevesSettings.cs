using Jeeves.Common;

namespace Jeeves.Core
{
    public class JeevesSettings
    {
        public JeevesSettings(bool useHttps)
        {
            UseHttps = useHttps;
        }

        public bool UseHttps { get; }
    }
}
