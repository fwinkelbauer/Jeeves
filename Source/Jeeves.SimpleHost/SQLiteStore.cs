using System;
using System.Data.SQLite;
using System.IO;
using Jeeves.Core;

namespace Jeeves.SimpleHost
{
    internal class SQLiteStore : IDataStore
    {
        private readonly FileInfo _database;
        private readonly string _connectionString;

        // TODO fw optimise queries? (e.g. use hashes)
        // TODO fw improve using clutter
        // TODO fw use something like EF Code First?
        public SQLiteStore(FileInfo database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _connectionString = $"Data Source = {_database}";

            if (!_database.Exists)
            {
                SQLiteConnection.CreateFile(_database.FullName);

                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    using (var cmd = new SQLiteCommand(connection))
                    {
                        cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS Configuration
(
  ID            INTEGER     PRIMARY KEY,
  Application   TEXT        COLLATE NOCASE NOT NULL,
  Host          TEXT        COLLATE NOCASE NOT NULL,
  Revision      INTEGER     NOT NULL,
  Key           TEXT        COLLATE NOCASE NOT NULL,
  Value         TEXT        NOT NULL
);

CREATE TABLE IF NOT EXISTS User
(
  Apikey        TEXT        PRIMARY KEY,
  UserName      TEXT        COLLATE NOCASE NOT NULL,
  Application   TEXT        COLLATE NOCASE NOT NULL,
  CanWrite      BOOLEAN     NOT NULL,
  IsAdmin       BOOLEAN     NOT NULL
);
";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
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
WHERE Application LIKE @App
AND Host LIKE @Host OR Host is NULL
AND Key like @Key
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
WHERE Apikey LIKE @Apikey";

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
