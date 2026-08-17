using Microsoft.Data.Sqlite;

namespace sushi_restaurant_project.Shared.Database
{
    public sealed class DatabaseSeeder
    {
        private readonly SushiDatabase _database;

        public DatabaseSeeder(SushiDatabase database)
        {
            _database = database;
        }

        public void Seed()
        {
            using var connection = _database.CreateConnection();
            connection.Open();

            // Il seed deve essere eseguito solo su un database appena creato.
            using var countCommand = connection.CreateCommand();

            countCommand.CommandText =
                """
                SELECT COUNT(*)
                FROM Plates;
                """;

            var plateCount = Convert.ToInt32(
                countCommand.ExecuteScalar());

            if (plateCount > 0)
            {
                return;
            }

            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var plate in GetInitialPlates())
                {
                    InsertPlate(
                        connection,
                        transaction,
                        plate);

                    foreach (var allergen in plate.Allergens)
                    {
                        var allergenId = GetOrCreateAllergen(
                            connection,
                            transaction,
                            allergen);

                        InsertPlateAllergen(
                            connection,
                            transaction,
                            plate.Id,
                            allergenId);
                    }
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private static void InsertPlate(
            SqliteConnection connection,
            SqliteTransaction transaction,
            PlateSeedData plate)
        {
            using var command = connection.CreateCommand();

            command.Transaction = transaction;

            command.CommandText =
                """
                INSERT INTO Plates
                (
                    Id,
                    Code,
                    Category,
                    Name,
                    Description,
                    DetailedDescription,
                    UrlImage,
                    PriceCents,
                    IsFrozen
                )
                VALUES
                (
                    @Id,
                    @Code,
                    @Category,
                    @Name,
                    @Description,
                    @DetailedDescription,
                    @UrlImage,
                    @PriceCents,
                    @IsFrozen
                );
                """;

            command.Parameters.AddWithValue("@Id", plate.Id);
            command.Parameters.AddWithValue("@Code", plate.Code);
            command.Parameters.AddWithValue("@Category", plate.Category);
            command.Parameters.AddWithValue("@Name", plate.Name);
            command.Parameters.AddWithValue(
                "@Description",
                plate.Description);

            command.Parameters.AddWithValue(
                "@DetailedDescription",
                plate.DetailedDescription);

            command.Parameters.AddWithValue(
                "@UrlImage",
                plate.UrlImage);

            command.Parameters.AddWithValue(
                "@PriceCents",
                plate.PriceCents);

            command.Parameters.AddWithValue(
                "@IsFrozen",
                plate.IsFrozen ? 1 : 0);

            command.ExecuteNonQuery();
        }

        private static long GetOrCreateAllergen(
            SqliteConnection connection,
            SqliteTransaction transaction,
            string allergenName)
        {
            using var insertCommand = connection.CreateCommand();

            insertCommand.Transaction = transaction;

            insertCommand.CommandText =
                """
                INSERT OR IGNORE INTO Allergens (Name)
                VALUES (@Name);
                """;

            insertCommand.Parameters.AddWithValue(
                "@Name",
                allergenName);

            insertCommand.ExecuteNonQuery();

            using var selectCommand = connection.CreateCommand();

            selectCommand.Transaction = transaction;

            selectCommand.CommandText =
                """
                SELECT Id
                FROM Allergens
                WHERE Name = @Name;
                """;

            selectCommand.Parameters.AddWithValue(
                "@Name",
                allergenName);

            return Convert.ToInt64(
                selectCommand.ExecuteScalar());
        }

        private static void InsertPlateAllergen(
            SqliteConnection connection,
            SqliteTransaction transaction,
            int plateId,
            long allergenId)
        {
            using var command = connection.CreateCommand();

            command.Transaction = transaction;

            command.CommandText =
                """
                INSERT INTO PlateAllergens
                (
                    PlateId,
                    AllergenId
                )
                VALUES
                (
                    @PlateId,
                    @AllergenId
                );
                """;

            command.Parameters.AddWithValue(
                "@PlateId",
                plateId);

            command.Parameters.AddWithValue(
                "@AllergenId",
                allergenId);

            command.ExecuteNonQuery();
        }

        private static List<PlateSeedData> GetInitialPlates()
        {
            return
            [
                new(
                    1,
                    "ANT-001",
                    "Antipasti",
                    "Edamame",
                    "Baccelli di soia al vapore con sale.",
                    "Baccelli di soia edamame cotti delicatamente al vapore e serviti con una leggera spolverata di sale. Un antipasto semplice e vegetale, caratterizzato da una consistenza tenera e da un gusto fresco e leggermente sapido.",
                    "image/edamame.png",
                    400,
                    false,
                    ["Soia"]
                ),

                new(
                    2,
                    "ANT-002",
                    "Antipasti",
                    "Gyoza di carne",
                    "Ravioli giapponesi ripieni di carne e verdure.",
                    "Ravioli giapponesi dalla sfoglia sottile, farciti con un ripieno saporito di carne e verdure. La consistenza morbida dell'interno si unisce a quella più compatta dell'involucro, creando un antipasto ricco e aromatico.",
                    "image/gyoza-carne.png",
                    550,
                    true,
                    ["Glutine", "Soia"]
                ),

                new(
                    3,
                    "NIG-001",
                    "Nigiri",
                    "Nigiri salmone",
                    "Riso condito con una fettina di salmone.",
                    "Bocconcino di riso condito, modellato e completato con una fettina di salmone. Il sapore delicato e naturalmente ricco del pesce si combina con la morbidezza del riso, dando vita a un assaggio semplice ed equilibrato.",
                    "image/nigiri-salmone.png",
                    350,
                    true,
                    ["Pesce"]
                ),

                new(
                    4,
                    "NIG-002",
                    "Nigiri",
                    "Nigiri tonno",
                    "Riso condito con una fettina di tonno.",
                    "Bocconcino di riso condito, modellato e ricoperto con una fettina di tonno. La consistenza compatta del pesce e il suo gusto deciso si abbinano alla morbidezza del riso in una preparazione essenziale e tradizionale.",
                    "image/nigiri-tonno.png",
                    400,
                    true,
                    ["Pesce"]
                ),

                new(
                    5,
                    "URA-001",
                    "Uramaki",
                    "Uramaki salmone e avocado",
                    "Roll con salmone, avocado e semi di sesamo.",
                    "Roll di riso con salmone e avocado, completato da semi di sesamo. La morbidezza del pesce e la cremosità dell'avocado si bilanciano con la nota leggermente tostata del sesamo, creando un uramaki delicato e armonioso.",
                    "image/uramaki-salmone-avocado.png",
                    850,
                    true,
                    ["Pesce", "Sesamo"]
                ),

                new(
                    6,
                    "URA-002",
                    "Uramaki",
                    "Uramaki spicy tuna",
                    "Roll con tonno, salsa piccante e semi di sesamo.",
                    "Roll di riso con tonno, salsa piccante e semi di sesamo. Il gusto intenso del tonno viene esaltato dalla nota vivace della salsa e dal profumo leggermente tostato del sesamo, per un uramaki dal carattere deciso.",
                    "image/uramaki-spicy-tuna.png",
                    900,
                    true,
                    ["Pesce", "Sesamo", "Soia"]
                ),

                new(
                    7,
                    "BEV-001",
                    "Bevande",
                    "Acqua naturale",
                    "Bottiglia di acqua naturale da 50 cl.",
                    "Bottiglia da 50 cl di acqua naturale, ideale per accompagnare il pasto con un gusto neutro e leggero. Servita fresca, permette di apprezzare pienamente i sapori delle diverse portate.",
                    "image/acqua-naturale.png",
                    200,
                    false,
                    []
                ),

                new(
                    8,
                    "BEV-002",
                    "Bevande",
                    "Tè verde",
                    "Tè verde giapponese servito caldo.",
                    "Tè verde giapponese servito caldo, caratterizzato da un profilo aromatico vegetale e delicato. Una bevanda piacevole e avvolgente, adatta ad accompagnare il pasto o a concluderlo con una nota leggera.",
                    "image/te-verde.png",
                    300,
                    false,
                    []
                ),

                new(
                    9,
                    "DES-001",
                    "Dessert",
                    "Mochi al mango",
                    "Dolce di riso con ripieno cremoso al mango.",
                    "Dolce dalla morbida pasta di riso, farcito con un ripieno cremoso al mango. La consistenza elastica dell'involucro racchiude una crema fresca e fruttata, creando un dessert delicato dal gusto tropicale.",
                    "image/mochi-mango.png",
                    450,
                    true,
                    ["Latte"]
                ),

                new(
                    10,
                    "DES-002",
                    "Dessert",
                    "Dorayaki",
                    "Dolce giapponese farcito con crema di fagioli rossi.",
                    "Dolce giapponese composto da due soffici dischi che racchiudono una crema di fagioli rossi. Il ripieno morbido e dolce si abbina alla consistenza delicata dell'impasto, offrendo un dessert tradizionale e avvolgente.",
                    "image/dorayaki.png",
                    400,
                    false,
                    ["Glutine", "Uova"]
                )
            ];
        }

        private sealed record PlateSeedData(
            int Id,
            string Code,
            string Category,
            string Name,
            string Description,
            string DetailedDescription,
            string UrlImage,
            int PriceCents,
            bool IsFrozen,
            string[] Allergens);
    }
}