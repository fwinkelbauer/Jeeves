using System.Data.SQLite;
using System.IO;
using Dapper;
using Jeeves.Core;

namespace Jeeves
{
    internal class SQLiteStore : IDataStore
    {
        #region Queries

        private const string SelectConfigurationQuery = @"
SELECT Value
FROM Configuration
WHERE (UserName = @User OR UserName = '')
AND Application = @App
AND Key = @Key
ORDER BY UserName DESC, ID DESC
LIMIT 1;";

        private const string InsertConfigurationQuery = @"
INSERT INTO Configuration (UserName, Application, Key, Value)
VALUES (@User, @App, @Key, @Value);";

        private const string SelectUserQuery = @"
SELECT UserName, Application, CanWrite
FROM User
WHERE Apikey = @Apikey;";

        #endregion

        private readonly string _connectionString;

        public SQLiteStore(FileInfo database)
        {
            _connectionString = $"Data Source = {database}";
        }

        public string RetrieveValue(string userName, string application, string key)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                return connection.QueryFirst<string>(SelectConfigurationQuery, new { User = userName, App = application, Key = key });
            }
        }

        public void PutValue(string userName, string application, string key, string value)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                connection.Execute(InsertConfigurationQuery, new { User = userName, App = application, Key = key, Value = value });
            }
        }

        public JeevesUser RetrieveUser(string apikey)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                return connection.QueryFirst<JeevesUser>(SelectUserQuery, new { Apikey = apikey });
            }
        }
    }
}
