using System;
using EntryPoint;
using Serilog;

namespace Jeeves
{
    public static class Program
    {
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
                var settings = AppSettings.Load();
                var commands = new CliCommands(settings);

                Cli.Execute(commands, args);
            }
            catch (Exception e)
            {
                Log.Error(e, "Could not start Jeeves");
                Environment.Exit(-1);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
