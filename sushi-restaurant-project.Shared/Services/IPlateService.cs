using sushi_restaurant_project.Shared.Models;

namespace sushi_restaurant_project.Shared.Services
{
    public interface IPlateService
    {
        Task<List<Plate>> GetPlatesAsync();
    }
}