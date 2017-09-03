using System;
using System.IO;
using System.Threading;
using Jeeves.Core;
using Serilog;

namespace Jeeves
{
    internal sealed class Service : IDisposable
    {
        private static readonly object Lock = new object();

        private readonly ILogger _log = Log.ForContext<Service>();

        private readonly FileInfo _database;
        private readonly Uri _baseUrl;
        private readonly JeevesHost _host;
        private readonly string _sqlScriptsFolder;

        public Service(FileInfo database, Uri url, JeevesSettings settings, string sqlScriptsFolder)
        {
            _database = database;
            _baseUrl = url;
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
                    new CommandMigrate().MigrateDatabase(_database, _sqlScriptsFolder);
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

        private class JeevesLog : IJeevesLog
        {
            private readonly ILogger _log = Log.ForContext("SourceContext", "Jeeves.Core");

            public void Debug(string messageTemplate, params object[] values)
            {
                _log.Debug(messageTemplate, values);
            }

            public void Error(Exception ex, string messageTemplate, params object[] values)
            {
                _log.Error(ex, messageTemplate, values);
            }
        }
    }
}
