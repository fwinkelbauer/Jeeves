using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using Dapper;
using DbUp;
using Jeeves.Core;
using Serilog;

namespace Jeeves
{
    internal class SQLiteStore : IDataStore, IUserAuthenticator
    {
        #region Queries

        private const string SelectConfigurationQuery = @"
SELECT Value
FROM Configuration
WHERE UserName = @User
AND Application = @App
AND Key = @Key
ORDER BY ID DESC
LIMIT 1;";

        private const string InsertConfigurationQuery = @"
INSERT INTO Configuration (UserName, Application, Key, Value, Created)
VALUES (@User, @App, @Key, @Value, @Created);";

        private const string SelectUserQuery = @"
SELECT UserName, Application
FROM User
WHERE Apikey = @Apikey;";

        #endregion

        private static readonly ILogger _log = Log.ForContext<SQLiteStore>();

        private readonly string _database;
        private readonly string _connectionString;

        private bool _migrateDone;

        public SQLiteStore(string database)
        {
            _database = database;
            _connectionString = $"Data Source = {database}";

            _migrateDone = false;
        }

        public string RetrieveValue(string userName, string application, string key)
        {
            Migrate();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var config = connection.QueryFirstOrDefault<Configuration>(SelectConfigurationQuery, new { User = userName, App = application, Key = key });

                return (config == null) ? null : config.Value;
            }
        }

        public void StoreValue(string userName, string application, string key, string value)
        {
            Migrate();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                connection.Execute(InsertConfigurationQuery, new { User = userName, App = application, Key = key, Value = value, Created = DateTime.Now });
            }
        }

        public JeevesUser RetrieveUser(string apikey)
        {
            Migrate();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                
                var user = connection.QueryFirstOrDefault<User>(SelectUserQuery, new { Apikey = apikey });

                return (user == null) ? null : new JeevesUser(user.UserName, user.Application);
            }
        }

        private void Migrate()
        {
            if (_migrateDone && File.Exists(_database))
            {
                return;
            }

            _migrateDone = false;

            var parentDir = Directory.GetParent(_database);

            if (!parentDir.Exists)
            {
                parentDir.Create();
            }

            var upgrader = DeployChanges.To
                .SQLiteDatabase(_connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .Build();

            var result = upgrader.PerformUpgrade();

            if (result.Successful)
            {
                _migrateDone = true;
            }
            else
            {
                _log.Error(result.Error, "Error while migrating database");
                throw result.Error;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
        private class Configuration
        {
            public Configuration(string value)
            {
                Value = value;
            }

            public string Value { get; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
        private class User
        {
            public User(string userName, string application)
            {
                UserName = userName;
                Application = application;
            }

            public string UserName { get; }

            public string Application { get; }
        }
    }
}
