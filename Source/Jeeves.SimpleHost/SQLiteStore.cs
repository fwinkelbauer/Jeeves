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
                        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Configuration
(
  ID            INTEGER     PRIMARY KEY,
  Application   TEXT        COLLATE NOCASE,
  Host          TEXT        COLLATE NOCASE,
  Revision      INTEGER,
  Key           TEXT        COLLATE NOCASE,
  Value         TEXT
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
SELECT Value FROM CONFIGURATION
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
            // TODO fw
            return JeevesUser.CreateAdmin("admin");
        }
    }
}
