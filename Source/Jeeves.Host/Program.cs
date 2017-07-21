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
        public static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.Seq(Settings.Default.SeqUrl)
                .CreateLogger();

            var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var database = new FileInfo(Path.Combine(userFolder, "Jeeves.sqlite"));
            var url = Settings.Default.BaseUrl;
            var settings = new JeevesSettings(Settings.Default.UseHttps, Settings.Default.UseAuthentication);

            HostFactory.Run(hc =>
            {
                hc.Service<Service>(sc =>
                {
                    sc.ConstructUsing(() => new Service(database, url, settings));
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
