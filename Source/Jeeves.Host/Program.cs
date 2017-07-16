using System;
using System.IO;
using System.Reflection;
using DbUp;
using Jeeves.Core;
using Jeeves.Host.Properties;

namespace Jeeves.Host
{
    public static class Program
    {
        public static void Main()
        {
            var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var database = new FileInfo(Path.Combine(userFolder, "Jeeves.sqlite"));

            MigrateDatabase(database);
            StartServer(database);
        }

        private static void MigrateDatabase(FileInfo database)
        {
            WriteColorLine($"Preparing database '{database}'", ConsoleColor.Magenta);

            var connectionString = $"Data Source = {database}";

            var upgrader = DeployChanges.To
                .SQLiteDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (result.Successful)
            {
                WriteColorLine("Finished migration!", ConsoleColor.Green);
            }
            else
            {
                WriteColorLine(result.Error.Message, ConsoleColor.Red);
                Console.WriteLine("Press ENTER to exit");
                Console.ReadLine();

                Environment.Exit(2);
            }
        }

        private static void StartServer(FileInfo database)
        {
            var url = Settings.Default.BaseUrl;
            var store = new SQLiteStore(database);
            var settings = new JeevesSettings(Settings.Default.UseHttps, Settings.Default.UseAuthentication);

            WriteColorLine($"Starting Jeeves on '{url}'", ConsoleColor.Magenta);

            using (var host = new JeevesHost(settings, store, new Uri(url)))
            {
                host.Start();
                Console.WriteLine("Press ENTER to exit");
                Console.ReadLine();
            }
        }

        private static void WriteColorLine(string line, ConsoleColor color)
        {
            var tmp = Console.ForegroundColor;

            Console.ForegroundColor = color;
            Console.WriteLine(line);
            Console.ForegroundColor = tmp;
        }
    }
}
