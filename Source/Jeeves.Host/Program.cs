using System;
using System.IO;
using Jeeves.Core;
using Jeeves.Host.Properties;
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
                .WriteTo.Seq(Settings.Default.SeqUrl)
                .CreateLogger();

            var database = new FileInfo(Settings.Default.Database);

            if (!database.Directory.Exists)
            {
                database.Directory.Create();
            }

            var url = Settings.Default.BaseUrl;
            var settings = new JeevesSettings(Settings.Default.UseHttps, Settings.Default.UseAuthentication);
            var sqlScriptsFolder = Settings.Default.SqlScriptsFolder;

            if (args != null && args.Length == 1 && args[0].Equals("migrate"))
            {
                try
                {
                    new CommandMigrate().MigrateDatabase(database, sqlScriptsFolder);
                    return;
                }
                catch (Exception)
                {
                    Environment.Exit(1);
                }
            }

            HostFactory.Run(hc =>
            {
                hc.Service<Service>(sc =>
                {
                    sc.ConstructUsing(() => new Service(database, url, settings, sqlScriptsFolder));
                    sc.WhenStarted(s => s.Start());
                    sc.WhenStopped(s =>
                    {
                        s.Stop();
                        s.Dispose();
                    });
                });

                hc.SetDescription("A simple REST service which provides configuration data");
                hc.SetServiceName("Jeeves.Host");
            });

            Log.CloseAndFlush();
        }
    }
}
