using System;
using Jeeves.Core;
using Serilog;
using Topshelf;

namespace Jeeves
{
    public static class Program
    {
        private const string SettingsPath = "settings.json";

        public static void Main()
        {
            try
            {
                var settings = SettingsLoader.Load(SettingsPath);
                var store = new SQLiteStore(settings.DatabasePath);

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.RollingFile(@"logs\{Date}.txt", retainedFileCountLimit: 7)
                    .CreateLogger();

                HostFactory.Run(hc =>
                {
                    hc.Service<JeevesHost>(sc =>
                    {
                        sc.ConstructUsing(() => new JeevesHost(
                            new JeevesSettings(settings.BaseUrl, settings.Security),
                            store,
                            new JeevesLog()));
                        sc.WhenStarted(s => s.Start());
                        sc.WhenStopped(s =>
                        {
                            s.Stop();
                            s.Dispose();
                        });
                    });

                    hc.SetServiceName(settings.ServiceName);
                    hc.SetDescription(settings.ServiceDescription);
                });

                Log.CloseAndFlush();
            }
            catch (Exception e)
            {
                Log.Error(e, "Could not start application");
                Log.CloseAndFlush();
                Environment.Exit(-1);
            }
        }
    }
}
