using System;
using System.Data.SQLite;
using System.IO;
using Jeeves.Core;

namespace Jeeves.Host
{
    internal class SQLiteStore : IDataStore
    {
        #region Queries

        private const string SelectConfigurationQuery = @"
SELECT Value FROM Configuration
WHERE Application = @App
AND (UserName = @UserName OR UserName = '')
AND Key = @Key
ORDER BY UserName DESC, ID DESC
LIMIT 1";

        private const string SelectUserQuery = @"
SELECT UserName, Application, CanWrite, IsAdmin FROM User
WHERE Apikey = @Apikey";

        #endregion

        private readonly FileInfo _database;
        private readonly string _connectionString;

        public SQLiteStore(FileInfo database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _connectionString = $"Data Source = {_database}";
        }

        public string RetrieveJson(string application, string userName, string key)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            using (var cmd = new SQLiteCommand(connection))
            {
                connection.Open();

                cmd.CommandText = SelectConfigurationQuery;

                cmd.Parameters.AddWithValue("@App", application);
                cmd.Parameters.AddWithValue("@UserName", userName);
                cmd.Parameters.AddWithValue("@Key", key);

                return (string)cmd.ExecuteScalar();
            }
        }

        public JeevesUser RetrieveUser(string apikey)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            using (var cmd = new SQLiteCommand(connection))
            {
                connection.Open();

                cmd.CommandText = SelectUserQuery;

                cmd.Parameters.AddWithValue("@Apikey", apikey);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var userName = reader["UserName"].ToString();
                        var isAdmin = Convert.ToBoolean(reader["IsAdmin"]);

                        if (isAdmin)
                        {
                            return JeevesUser.CreateAdmin(userName);
                        }
                        else
                        {
                            var applicationName = reader["Application"].ToString();
                            var canWrite = Convert.ToBoolean(reader["CanWrite"]);

                            return JeevesUser.CreateUser(userName, applicationName, canWrite);
                        }
                    }

                    return null;
                }
            }
        }
    }
}
