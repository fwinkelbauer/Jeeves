using System;
using System.IO;
using System.Reflection;
using DbUp;
using DbUp.Engine.Output;
using Jeeves.Core;
using Jeeves.Host.Properties;
using Serilog;

namespace Jeeves.Host
{
    public static class Program
    {
        public static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.Seq("http://127.0.0.1:5341")
                .CreateLogger();

            var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var database = new FileInfo(Path.Combine(userFolder, "Jeeves.sqlite"));

            try
            {
                MigrateDatabase(database);
                RunServer(database);
                Log.CloseAndFlush();
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred");
                Log.CloseAndFlush();
                Environment.Exit(2);
            }
        }

        private static void MigrateDatabase(FileInfo database)
        {
            Log.Debug("Preparing database {database}", database);

            var connectionString = $"Data Source = {database}";

            var upgrader = DeployChanges.To
                .SQLiteDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogTo(new DbUpLog())
                .Build();

            var result = upgrader.PerformUpgrade();

            if (result.Successful)
            {
                Log.Debug("Finished migration!");
            }
            else
            {
                throw result.Error;
            }
        }

        private static void RunServer(FileInfo database)
        {
            var url = Settings.Default.BaseUrl;
            var store = new SQLiteStore(database);
            var settings = new JeevesSettings(Settings.Default.UseHttps, Settings.Default.UseAuthentication);

            Log.Information("Starting Jeeves on {url}", url);

            using (var host = new JeevesHost(settings, store, new Uri(url)))
            {
                host.Start();
                Console.WriteLine("Press ENTER to exit");
                Console.ReadLine();
            }
        }

        private class DbUpLog : IUpgradeLog
        {
            private readonly ILogger _log;

            public DbUpLog()
            {
                _log = Log.ForContext("SourceContext", "DbUp");
            }

            public void WriteError(string format, params object[] args)
            {
                _log.Error(format, args);
            }

            public void WriteInformation(string format, params object[] args)
            {
                // I am using a "lower" log level by choice
                _log.Debug(format, args);
            }

            public void WriteWarning(string format, params object[] args)
            {
                _log.Warning(format, args);
            }
        }
    }
}
