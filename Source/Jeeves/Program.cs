using System;
using System.IO;
using Jeeves.Core;
using Serilog;
using Topshelf;

namespace Jeeves
{
    public static class Program
    {
        private const string JeevesPath = @"C:\ProgramData\Jeeves\";
        private const string SettingsPath = JeevesPath + "settings.json";
        private const string Database = JeevesPath + "Jeeves.sqlite";
        private const string SaltPath = JeevesPath + "salt";
        private const string LogFile = JeevesPath + @"Logs\{Date}.txt";

        public static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.RollingFile(LogFile, retainedFileCountLimit: 7)
                .CreateLogger();

            try
            {
                if (!Directory.Exists(JeevesPath))
                {
                    Directory.CreateDirectory(JeevesPath);
                }

                var settings = JeevesSettingsLoader.Load(SettingsPath);
                var salt = Hasher.LoadSalt(SaltPath);
                var store = new SQLiteStore(Database, salt);

                HostFactory.Run(hc =>
                {
                    hc.Service<JeevesHost>(sc =>
                    {
                        sc.ConstructUsing(() => new JeevesHost(settings, store, new JeevesLog()));
                        sc.WhenStarted(s => s.Start());
                        sc.WhenStopped(s =>
                        {
                            s.Stop();
                            s.Dispose();
                        });
                    });

                    hc.SetDescription("A simple REST service which provides configuration data for applications");
                    hc.SetServiceName("Jeeves");
                });

                Log.CloseAndFlush();
            }
            catch (Exception e)
            {
                Log.Error(e, "Could not start Jeeves");
                Log.CloseAndFlush();
                Environment.Exit(-1);
            }
        }
    }
}
