using sushi_restaurant_project.Shared.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace sushi_restaurant_project.Shared.Services
{
    public sealed class PlateService : IPlateService
    {
        public async Task<List<Plate>> GetPlatesAsync()
        {
            var assembly = typeof(PlateService).Assembly;

            var resourceName = assembly
                .GetManifestResourceNames()
                .FirstOrDefault(name =>
                    name.EndsWith(
                        "plates.mock.json",
                        StringComparison.OrdinalIgnoreCase));

            if (resourceName is null)
            {
                throw new InvalidOperationException(
                    "Il file plates.mock.json non è stato trovato tra le risorse incorporate.");
            }

            await using var stream =
                assembly.GetManifestResourceStream(resourceName);

            if (stream is null)
            {
                throw new InvalidOperationException(
                    "Non è stato possibile aprire plates.mock.json.");
            }

            var mockData =
                await JsonSerializer.DeserializeAsync<PlatesMockData>(stream);

            return mockData?.Plates ?? [];
        }

        private sealed class PlatesMockData
        {
            [JsonPropertyName("plates")]
            public List<Plate> Plates { get; set; } = [];
        }
    }
}