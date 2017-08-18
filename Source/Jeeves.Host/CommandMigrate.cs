using System.IO;
using System.Reflection;
using DbUp;
using DbUp.Engine.Output;
using Serilog;

namespace Jeeves.Host
{
    internal sealed class CommandMigrate
    {
        private readonly ILogger _log = Log.ForContext<CommandMigrate>();

        public void MigrateDatabase(FileInfo database, string sqlScriptsFolder)
        {
            _log.Debug("Preparing database {database}", database);

            var connectionString = $"Data Source = {database}";

            var dbUpBuilder = DeployChanges.To
                .SQLiteDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogTo(new DbUpLog());

            if (Directory.Exists(sqlScriptsFolder))
            {
                dbUpBuilder.WithScriptsFromFileSystem(sqlScriptsFolder);
            }

            var upgrader = dbUpBuilder.Build();

            var result = upgrader.PerformUpgrade();

            if (result.Successful)
            {
                _log.Debug("Finished migration!");
            }
            else
            {
                _log.Error(result.Error, "Error while migrating database");
                throw result.Error;
            }
        }

        private class DbUpLog : IUpgradeLog
        {
            private readonly ILogger _log = Log.ForContext("SourceContext", "DbUp");

            public void WriteError(string format, params object[] args)
            {
                _log.Error(format, args);
            }

            public void WriteInformation(string format, params object[] args)
            {
                // I am using a lower log level by choice
                _log.Debug(format, args);
            }

            public void WriteWarning(string format, params object[] args)
            {
                _log.Warning(format, args);
            }
        }
    }
}
