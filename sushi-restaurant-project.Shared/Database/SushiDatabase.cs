using Microsoft.Data.Sqlite;

namespace sushi_restaurant_project.Shared.Database
{
    public sealed class SushiDatabase
    {
        private readonly string _databasePath;

        public SushiDatabase(string databasePath)
        {
            _databasePath = databasePath;
        }

        public SqliteConnection CreateConnection()
        {
            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = _databasePath,
                ForeignKeys = true
            }.ToString();

            return new SqliteConnection(connectionString);
        }

        public void Initialize()
        {
            // Se la cartella destinata al database non esiste,
            // viene creata prima di aprire la connessione.
            var directory = Path.GetDirectoryName(_databasePath);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var connection = CreateConnection();

            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText =
                """
                CREATE TABLE IF NOT EXISTS Plates
                (
                    Id INTEGER PRIMARY KEY,
                    Code TEXT NOT NULL UNIQUE,
                    Category TEXT NOT NULL,
                    Name TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    DetailedDescription TEXT NOT NULL,
                    UrlImage TEXT NOT NULL,
                    PriceCents INTEGER NOT NULL CHECK (PriceCents >= 0),
                    IsFrozen INTEGER NOT NULL CHECK (IsFrozen IN (0, 1))
                );

                CREATE TABLE IF NOT EXISTS Allergens
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE
                );

                CREATE TABLE IF NOT EXISTS PlateAllergens
                (
                    PlateId INTEGER NOT NULL,
                    AllergenId INTEGER NOT NULL,

                    PRIMARY KEY (PlateId, AllergenId),

                    FOREIGN KEY (PlateId)
                        REFERENCES Plates(Id)
                        ON DELETE CASCADE,

                    FOREIGN KEY (AllergenId)
                        REFERENCES Allergens(Id)
                        ON DELETE CASCADE
                );
                """;

            command.ExecuteNonQuery();
        }
    }
}