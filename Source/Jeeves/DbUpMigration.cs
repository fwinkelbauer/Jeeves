﻿using System.IO;
using System.Reflection;
using DbUp;
using DbUp.Engine.Output;
using Serilog;

namespace Jeeves
{
    internal class DbUpMigration
    {
        private static readonly ILogger _log = Log.ForContext<DbUpMigration>();

        public static void Migrate(string database, string sqlScriptsFolder)
        {
            _log.Information("Preparing database {database}", database);

            var file = new FileInfo(database);

            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }

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
                _log.Information("Finished migration!");
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

            public void WriteError(string messageTemplate, params object[] args)
            {
                _log.Error(messageTemplate, args);
            }

            public void WriteInformation(string messageTemplate, params object[] args)
            {
                _log.Information(messageTemplate, args);
            }

            public void WriteWarning(string messageTemplate, params object[] args)
            {
                _log.Warning(messageTemplate, args);
            }
        }
    }
}
