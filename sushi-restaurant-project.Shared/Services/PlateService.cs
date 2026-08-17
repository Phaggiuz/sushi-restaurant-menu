using sushi_restaurant_project.Shared.Database;
using sushi_restaurant_project.Shared.Models;

namespace sushi_restaurant_project.Shared.Services
{
    public sealed class PlateService : IPlateService
    {
        private readonly SushiDatabase _database;

        public PlateService(SushiDatabase database)
        {
            _database = database;
        }

        public Task<List<Plate>> GetPlatesAsync()
        {
            using var connection = _database.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText =
                """
                SELECT
                    p.Id,
                    p.Code,
                    p.Category,
                    p.Name,
                    p.Description,
                    p.DetailedDescription,
                    p.UrlImage,
                    p.PriceCents,
                    p.IsFrozen,
                    a.Name AS Allergen
                FROM Plates p
                LEFT JOIN PlateAllergens pa
                    ON pa.PlateId = p.Id
                LEFT JOIN Allergens a
                    ON a.Id = pa.AllergenId
                ORDER BY p.Id, a.Name;
                """;

            using var reader = command.ExecuteReader();

            var plates = new Dictionary<int, Plate>();

            while (reader.Read())
            {
                var plateId = Convert.ToInt32(
                    reader.GetInt64(0));

                if (!plates.TryGetValue(plateId, out var plate))
                {
                    var priceCents = reader.GetInt64(7);

                    plate = new Plate
                    {
                        Id = plateId,
                        Code = reader.GetString(1),
                        Category = reader.GetString(2),
                        Name = reader.GetString(3),
                        Description = reader.GetString(4),
                        DetailedDescription = reader.GetString(5),
                        UrlImage = reader.GetString(6),
                        Price = priceCents / 100m,
                        IsFrozen = reader.GetInt64(8) == 1,
                        Allergens = []
                    };

                    plates.Add(plateId, plate);
                }

                if (!reader.IsDBNull(9))
                {
                    plate.Allergens.Add(
                        reader.GetString(9));
                }
            }

            return Task.FromResult(
                plates.Values.ToList());
        }
    }
}