namespace Jeeves.Core
{
    internal class ModuleSettings
    {
        public ModuleSettings(bool useHttps)
        {
            UseHttps = useHttps;
        }

        public bool UseHttps { get; }
    }
}
