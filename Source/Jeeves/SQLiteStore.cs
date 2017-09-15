using System;
using System.Data.SQLite;
using System.Reflection;
using Dapper;
using DbUp;
using DbUp.Engine.Output;
using Jeeves.Core;
using Serilog;

namespace Jeeves
{
    internal class SQLiteStore : IDataStore
    {
        #region Queries

        private const string SelectConfigurationQuery = @"
SELECT Value, Revoked
FROM Configuration
WHERE (UserName = @User OR UserName = '')
AND Application = @App
AND Key = @Key
ORDER BY UserName DESC, ID DESC
LIMIT 1;";

        private const string InsertConfigurationQuery = @"
INSERT INTO Configuration (UserName, Application, Key, Value, Revoked, Created)
VALUES (@User, @App, @Key, @Value, @Revoked, @Created);";

        private const string SelectUserQuery = @"
SELECT UserName, Application, CanWrite, Revoked
FROM User
WHERE Apikey = @Apikey;";

        #endregion

        private static readonly ILogger _log = Log.ForContext<SQLiteStore>();

        private readonly string _database;
        private readonly string _connectionString;
        private readonly string _salt;

        private bool _migrateDone;

        public SQLiteStore(string database, string salt)
        {
            _database = database;
            _connectionString = $"Data Source = {database}";
            _salt = salt;

            _migrateDone = false;
        }

        public string RetrieveValue(string userName, string application, string key)
        {
            Migrate();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var config = connection.QueryFirst<Configuration>(SelectConfigurationQuery, new { User = userName, App = application, Key = key });

                return config.Revoked ? null : config.Value;
            }
        }

        public void StoreValue(string userName, string application, string key, string value)
        {
            Migrate();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                connection.Execute(InsertConfigurationQuery, new { User = userName, App = application, Key = key, Value = value, Revoked = false, Created = DateTime.Now });
            }
        }

        public JeevesUser RetrieveUser(string apikey)
        {
            Migrate();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var user = connection.QueryFirst<User>(SelectUserQuery, new { Apikey = Hasher.Hash(apikey, _salt) });

                return user.Revoked ? null : new JeevesUser(user.UserName, user.Application, user.CanWrite);
            }
        }

        private void Migrate()
        {
            if (_migrateDone)
            {
                return;
            }

            _log.Information("Preparing database {database}", _database);

            var upgrader = DeployChanges.To
                .SQLiteDatabase(_connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogTo(new DbUpLog())
                .Build();

            var result = upgrader.PerformUpgrade();

            if (result.Successful)
            {
                _migrateDone = true;
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
            private readonly ILogger _dbUpLog = Log.ForContext("SourceContext", "DbUp");

            public void WriteError(string messageTemplate, params object[] args)
            {
                _dbUpLog.Error(messageTemplate, args);
            }

            public void WriteInformation(string messageTemplate, params object[] args)
            {
                _dbUpLog.Information(messageTemplate, args);
            }

            public void WriteWarning(string messageTemplate, params object[] args)
            {
                _dbUpLog.Warning(messageTemplate, args);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
        private class Configuration
        {
            public Configuration(string value, bool revoked)
            {
                Value = value;
                Revoked = revoked;
            }

            public string Value { get; }

            public bool Revoked { get; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
        private class User
        {
            public User(string userName, string application, bool canWrite, bool revoked)
            {
                UserName = userName;
                Application = application;
                CanWrite = canWrite;
                Revoked = revoked;
            }

            public string UserName { get; }

            public string Application { get; }

            public bool CanWrite { get; }

            public bool Revoked { get; }
        }
    }
}
