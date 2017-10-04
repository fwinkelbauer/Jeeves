using System;
using System.IO;
using Jeeves.Core;
using Serilog;
using Topshelf;

namespace Jeeves
{
    public static class Program
    {
        private const string BaseUrl = "http://localhost:9042/jeeves/";
        private const string JeevesDir = @"C:\ProgramData\Jeeves";

        private static readonly string _databasePath = Path.Combine(JeevesDir, "Jeeves.sqlite");
        private static readonly string _logPath = Path.Combine(JeevesDir, @"logs\{Date}.txt");

        public static void Main()
        {
            var exitCode = HostFactory.Run(hc =>
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

            Environment.Exit((int)exitCode);
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
                    .WriteTo.RollingFile(_logPath, retainedFileCountLimit: 7)
                    .CreateLogger();

                var store = new SQLiteStore(_databasePath);

                _host = new JeevesHostBuilder(new Uri(BaseUrl), store)
                    .WithUserAuthentication(store)
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
        }
    }
}
