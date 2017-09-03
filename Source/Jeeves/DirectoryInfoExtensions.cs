using System.IO;

namespace Jeeves
{
    internal static class DirectoryInfoExtensions
    {
        public static void EnsureExists(this DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                directory.Create();
            }
        }
    }
}
