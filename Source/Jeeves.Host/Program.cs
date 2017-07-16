using System;
using System.IO;
using Jeeves.Core;
using Jeeves.Host.Properties;

namespace Jeeves.Host
{
    public static class Program
    {
        public static void Main()
        {
            var settings = new JeevesSettings(Settings.Default.UseHttps, Settings.Default.UseAuthentication);

            var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var file = new FileInfo(Path.Combine(userFolder, "Jeeves.sqlite"));
            var store = new SQLiteStore(file);

            using (var host = new JeevesHost(settings, store, new Uri(Settings.Default.BaseUrl)))
            {
                host.Start();
                Console.WriteLine("Press ENTER to exit");
                Console.ReadLine();
            }
        }
    }
}
