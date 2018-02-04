using System;
using System.IO;
using Jeeves.Core;
using Newtonsoft.Json;
using Serilog;
using Topshelf;

namespace Jeeves
{
    public static class Program
    {
        private const string SettingsFile = "Settings.json";

        public static void Main()
        {
            var exitCode = HostFactory.Run(hc =>
            {
                hc.Service<Service>();
                hc.SetServiceName("Jeeves");
                hc.SetDescription("A simple REST service which provides configuration data for applications");
            });

            Environment.Exit((int)exitCode);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
        private class Service : ServiceControl
        {
            private JeevesHost _host;

            public bool Start(HostControl hostControl)
            {
                Settings settings = null;
                var logConfig = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console();

                try
                {
                    settings = LoadSettings();
                    Log.Logger = logConfig
                        .WriteTo.RollingFile(Path.Combine(settings.LogDirectory, "{Date}.txt"), retainedFileCountLimit: 7)
                        .CreateLogger();
                }
                catch (Exception e)
                {
                    Log.Logger = logConfig.CreateLogger();
                    Log.Error(e, "Error while loading settings");
                    return false;
                }

                var store = new SQLiteStore(settings.DatabaseFile);

                _host = new JeevesHostBuilder(new Uri(settings.BaseUrl), store)
                    .LogTo(new JeevesLog())
                    .Build();

                _host.Start();

                return true;
            }

            public bool Stop(HostControl hostControl)
            {
                _host?.Stop();
                _host?.Dispose();

                Log.CloseAndFlush();

                return true;
            }

            private Settings LoadSettings()
            {
                if (File.Exists(SettingsFile))
                {
                    return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsFile));
                }

                return new Settings("http://localhost:9042/jeeves/", "Jeeves.sqlite", "logs");
            }
        }
    }
}
