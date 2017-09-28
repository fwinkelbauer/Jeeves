﻿using Jeeves.Core;
using Serilog;
using Topshelf;

namespace Jeeves
{
    public static class Program
    {
        private const string SettingsPath = "settings.json";

        public static void Main()
        {
            HostFactory.Run(hc =>
            {
                hc.Service<Service>();

                hc.OnException(e =>
                {
                    Log.Error(e, "Could not start Jeeves");
                    Log.CloseAndFlush();
                });

                hc.SetServiceName("Jeeves");
                hc.SetDescription("A simple REST service which provides configuration data for applications");
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
        private class Service : ServiceControl
        {
            private JeevesHost _host;

            public bool Start(HostControl hostControl)
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.RollingFile("logs/{Date}.txt", retainedFileCountLimit: 7)
                    .CreateLogger();

                var settings = SettingsLoader.Load(SettingsPath);
                _host = new JeevesHost(
                    new JeevesSettings(settings.BaseUrl, settings.Security),
                    new SQLiteStore(settings.DatabasePath),
                    new JeevesLog());

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
        }
    }
}
