using System;
using System.IO;
using System.Reflection;
using DbUp;
using DbUp.Engine.Output;
using Jeeves.Core;
using Serilog;

namespace Jeeves.Host
{
    internal sealed class Service : IDisposable
    {
        private static readonly ILogger _log = Log.ForContext<Service>();

        private readonly FileInfo _database;
        private readonly Uri _baseUrl;
        private readonly JeevesHost _host;

        public Service(FileInfo database, string url, JeevesSettings settings)
        {
            _database = database;
            _baseUrl = new Uri(url);
            _host = new JeevesHost(_baseUrl, settings, new SQLiteStore(database), new JeevesLog());
        }

        public void Dispose()
        {
            _host.Dispose();
        }

        public void Start()
        {
            _log.Information("Starting Jeeves");
            MigrateDatabase();
            _log.Information("Starting web service on {url}", _baseUrl);
            _host.Start();
        }

        public void Stop()
        {
            _log.Information("Stopping web service on {url}", _baseUrl);
            _host.Stop();
            _log.Information("Goodbye");
        }

        private void MigrateDatabase()
        {
            _log.Debug("Preparing database {database}", _database);

            var connectionString = $"Data Source = {_database}";

            var upgrader = DeployChanges.To
                .SQLiteDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogTo(new DbUpLog())
                .Build();

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
            private static readonly ILogger _log = Log.ForContext("SourceContext", "DbUp");

            public void WriteError(string format, params object[] args)
            {
                _log.Error(format, args);
            }

            public void WriteInformation(string format, params object[] args)
            {
                // I am using a "lower" log level by choice
                _log.Debug(format, args);
            }

            public void WriteWarning(string format, params object[] args)
            {
                _log.Warning(format, args);
            }
        }

        private class JeevesLog : IJeevesLog
        {
            private static readonly ILogger _log = Log.ForContext("SourceContext", "Jeeves.Core");

            public void DebugFormat(string format, params object[] values)
            {
                _log.Debug(format, values);
            }

            public void ErrorFormat(Exception ex, string format, params object[] values)
            {
                _log.Error(ex, format, values);
            }
        }
    }
}
