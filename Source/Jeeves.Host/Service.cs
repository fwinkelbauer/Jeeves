using System;
using System.IO;
using System.Reflection;
using System.Threading;
using DbUp;
using DbUp.Engine.Output;
using Jeeves.Core;
using Serilog;

namespace Jeeves.Host
{
    internal sealed class Service : IDisposable
    {
        private static readonly object Lock = new object();

        private readonly ILogger _log = Log.ForContext<Service>();

        private readonly FileInfo _database;
        private readonly Uri _baseUrl;
        private readonly JeevesHost _host;
        private readonly string _sqlScriptsFolder;

        public Service(FileInfo database, string url, JeevesSettings settings, string sqlScriptsFolder)
        {
            _database = database;
            _baseUrl = new Uri(url);
            _host = new JeevesHost(_baseUrl, settings, new SQLiteStore(database), new JeevesLog());
            _sqlScriptsFolder = sqlScriptsFolder;
        }

        public void Dispose()
        {
            _host.Dispose();
        }

        public void Start()
        {
            new Thread(() =>
            {
                lock (Lock)
                {
                    _log.Information("Starting Jeeves");
                    MigrateDatabase();
                    _log.Information("Starting web service on {url}", _baseUrl);
                    _host.Start();
                }
            }).Start();
        }

        public void Stop()
        {
            lock (Lock)
            {
                _log.Information("Stopping web service on {url}", _baseUrl);
                _host.Stop();
                _log.Information("Goodbye");
            }
        }

        private void MigrateDatabase()
        {
            _log.Debug("Preparing database {database}", _database);

            var connectionString = $"Data Source = {_database}";

            var dbUpBuilder = DeployChanges.To
                .SQLiteDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogTo(new DbUpLog());

            if (Directory.Exists(_sqlScriptsFolder))
            {
                dbUpBuilder.WithScriptsFromFileSystem(_sqlScriptsFolder);
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
