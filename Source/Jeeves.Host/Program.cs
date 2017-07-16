using System;
using System.IO;
using Jeeves.Core;

namespace Jeeves.Host
{
    public static class Program
    {
        public static void Main()
        {
            var settings = new JeevesSettings(false, true);

            var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var file = new FileInfo(Path.Combine(userFolder, "Jeeves.sqlite"));
            var store = new SQLiteStore(file);

            using (var host = new JeevesHost(settings, store, new Uri("http://localhost:9042/jeeves/")))
            {
                host.Start();
                Console.WriteLine("Press ENTER to exit");
                Console.ReadLine();
            }
        }
    }
}
