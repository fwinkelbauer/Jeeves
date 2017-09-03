using System;
using Jeeves.Core;
using Serilog;
using Topshelf;

namespace Jeeves.Host
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.RollingFile(@"C:\ProgramData\Jeeves\Logs\{Date}.txt", retainedFileCountLimit: 7)
                .CreateLogger();

            try
            {
                Start(args);
            }
            catch (Exception e)
            {
                Log.Error(e, "Could not start Jeeves.Host");
                Environment.Exit(-1);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void Start(string[] args)
        {
            var settings = AppSettings.Load();
            var database = settings.Database;
            database.Directory.EnsureExists();

            var sqlScriptsFolder = settings.SqlScriptsDirectory;

            if (args != null && args.Length == 1 && args[0].Equals("migrate"))
            {
                new CommandMigrate().MigrateDatabase(database, sqlScriptsFolder);
            }
            else
            {
                HostFactory.Run(hc =>
                {
                    hc.Service<Service>(sc =>
                    {
                        sc.ConstructUsing(() => new Service(
                            database,
                            settings.BaseUrl,
                            new JeevesSettings(settings.UseHttps, settings.UseAuthentication),
                            sqlScriptsFolder));
                        sc.WhenStarted(s => s.Start());
                        sc.WhenStopped(s =>
                        {
                            s.Stop();
                            s.Dispose();
                        });
                    });

                    hc.SetDescription("A simple REST service which provides configuration data for applications");
                    hc.SetServiceName("Jeeves.Host");
                });
            }
        }
    }
}
