using System;
using System.Data.SQLite;
using System.IO;
using Jeeves.Core;

namespace Jeeves.Host
{
    internal class SQLiteStore : IDataStore
    {
        private readonly FileInfo _database;
        private readonly string _connectionString;

        // TODO fw optimise queries? (e.g. use hashes)
        // TODO fw improve using clutter
        public SQLiteStore(FileInfo database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _connectionString = $"Data Source = {_database}";
        }

        public string RetrieveJson(string application, string host, string key)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (var cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = @"
SELECT Value FROM Configuration
WHERE Application = @App
AND Host = @Host OR Host = ''
AND Key = @Key
ORDER BY Host DESC, Revision DESC
LIMIT 1";

                    cmd.Parameters.AddWithValue("@App", application);
                    cmd.Parameters.AddWithValue("@Host", host);
                    cmd.Parameters.AddWithValue("@Key", key);

                    return (string)cmd.ExecuteScalar();
                }
            }
        }

        public JeevesUser RetrieveUser(string apikey)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (var cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = @"
SELECT UserName, Application, CanWrite, IsAdmin FROM User
WHERE Apikey = @Apikey";

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
}
