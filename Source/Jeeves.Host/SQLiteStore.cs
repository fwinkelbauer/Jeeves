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
            _connectionString = $"Data Source = {database.ThrowIfNull(nameof(database))}";
        }

        public string RetrieveValue(string userName, string application, string key)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            using (var cmd = new SQLiteCommand(connection))
            {
                connection.Open();

                cmd.CommandText = SelectConfigurationQuery;

                cmd.Parameters.AddWithValue("@User", userName);
                cmd.Parameters.AddWithValue("@App", application);
                cmd.Parameters.AddWithValue("@Key", key);

                return (string)cmd.ExecuteScalar();
            }
        }

        public void PutValue(string userName, string application, string key, string value)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            using (var cmd = new SQLiteCommand(connection))
            {
                connection.Open();

                cmd.CommandText = InsertConfigurationQuery;

                cmd.Parameters.AddWithValue("@User", userName);
                cmd.Parameters.AddWithValue("@App", application);
                cmd.Parameters.AddWithValue("@Key", key);
                cmd.Parameters.AddWithValue("@Value", value);

                cmd.ExecuteNonQuery();
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
                        var applicationName = reader["Application"].ToString();
                        var canWrite = Convert.ToBoolean(reader["CanWrite"]);

                        return new JeevesUser(userName, applicationName, canWrite);
                    }

                    return null;
                }
            }
        }
    }
}
