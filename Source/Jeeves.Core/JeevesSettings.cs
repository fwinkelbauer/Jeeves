using Jeeves.Common;

namespace Jeeves.Core
{
    public class JeevesSettings
    {
        public JeevesSettings(bool useHttps, string tokenSignatureKey)
        {
            UseHttps = useHttps;
            TokenSignatureKey = tokenSignatureKey.ThrowIfNull(nameof(tokenSignatureKey));
        }

        public bool UseHttps { get; }

        public string TokenSignatureKey { get; }
    }
}
