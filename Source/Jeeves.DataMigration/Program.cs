using System;
using System.IO;
using System.Reflection;
using DbUp;

namespace Jeeves.DataMigration
{
    public static class Program
    {
        public static int Main()
        {
            var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var file = new FileInfo(Path.Combine(userFolder, "Jeeves.sqlite"));
            var connectionString = $"Data Source = {file}";

            var upgrader = DeployChanges.To
                .SQLiteDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Success!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
            }

            Console.ResetColor();
            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();

            return result.Successful ? 0 : -1;
        }
    }
}
