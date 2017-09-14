using System;
using Serilog;

namespace Jeeves
{
    public static class Program
    {
        private const string SettingsPath = @"C:\ProgramData\Jeeves\settings.json";
        private const string Database = @"C:\ProgramData\Jeeves\Jeeves.sqlite";
        private const string SqlScritpsDirectory = @"C:\ProgramData\Jeeves\Scripts";
        private const string LogFile = @"C:\ProgramData\Jeeves\Logs\{Date}.txt";

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.RollingFile(LogFile, retainedFileCountLimit: 7)
                .CreateLogger();

            try
            {
                var settings = JeevesSettingsLoader.Load(SettingsPath);

                Topshelf.Start(Database, SqlScritpsDirectory, settings);
            }
            catch (Exception e)
            {
                Log.Error(e, "Could not start Jeeves");
                Log.CloseAndFlush();
                Environment.Exit(-1);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
