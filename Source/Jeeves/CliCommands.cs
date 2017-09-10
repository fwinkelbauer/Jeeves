using System;
using System.Text;
using EntryPoint;

namespace Jeeves
{
    internal class CliCommands : BaseCliCommands
    {
        private readonly AppSettings _settings;

        public CliCommands(AppSettings settings)
        {
            _settings = settings;
        }

        [Command("migrate")]
        [Help("Executes new SQL scripts")]
        public void Migrate(string[] args)
        {
            DbUpMigration.Migrate(_settings.Database, _settings.SqlScriptsDirectory);
        }

        [Command("install")]
        [Help("Install Jeeves as a service")]
        public void Install(string[] args)
        {
            Topshelf.Start(AppSettings.Load());
        }

        [Command("uninstall")]
        [Help("Uninstalls the Jeeves service")]
        public void Uninstall(string[] args)
        {
            Topshelf.Start(AppSettings.Load());
        }

        [DefaultCommand]
        [Command("start")]
        [Help("Starts the Jeeves service")]
        public void Start(string[] args)
        {
            Topshelf.Start(AppSettings.Load());
        }

        [Command("stop")]
        [Help("Stops the Jeeves service")]
        public void stop(string[] args)
        {
            Topshelf.Start(AppSettings.Load());
        }

        [Command("reset")]
        [Help("Resets Jeeves configuration")]
        public void ResetConfiguration(string[] args)
        {
            AppSettings.WriteDefault();
        }

        [Command("show")]
        [Help("Prints the current configuration")]
        public void ShowConfiguration(string[] args)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"Base URL: {_settings.BaseUrl}");
            builder.AppendLine($"Database: {_settings.Database}");
            builder.AppendLine($"SQL scripts directory: {_settings.SqlScriptsDirectory}");
            builder.AppendLine($"Https enabled: {_settings.UseHttps}");
            builder.AppendLine($"Authentication enabled: {_settings.UseAuthentication}");

            Console.WriteLine(builder.ToString());
        }

        [Command("config")]
        [Help("Changes Jeeves configuration")]
        public void SetConfiguration(string[] args)
        {
            var options = new ConfigCliArguments(_settings);
            options = Cli.Parse(options, args);

            AppSettings.Write(new AppSettings(
                options.BaseUrl,
                options.UseHttps,
                options.UseAuthentication,
                options.Database,
                options.SqlScriptsDirectory));

            Console.WriteLine("Note: Any settings change will only be loaded after restarting Jeeves");
        }

        public override void OnHelpInvoked(string helpText)
        {
            Console.WriteLine(helpText);
        }

        private class ConfigCliArguments : BaseCliArguments
        {
            public ConfigCliArguments(AppSettings settings) : base("Configuration")
            {
                BaseUrl = settings.BaseUrl;
                UseHttps = settings.UseHttps;
                UseAuthentication = settings.UseAuthentication;
                Database = settings.Database;
                SqlScriptsDirectory = settings.SqlScriptsDirectory;
            }

            [OptionParameter("baseUrl")]
            public string BaseUrl { get; set; }

            [OptionParameter("useHttps")]
            public bool UseHttps { get; set; }

            [OptionParameter("useAuthentication")]
            public bool UseAuthentication { get; set; }

            [OptionParameter("database")]
            public string Database { get; set; }

            [OptionParameter("sqlScriptsDirectory")]
            public string SqlScriptsDirectory { get; set; }
        }
    }
}
