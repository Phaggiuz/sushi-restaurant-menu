namespace sushi_restaurant_project.Shared.Models
{
    public class Plate
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlImage { get; set; }
        public decimal Price { get; set; }
        public bool IsFrozen { get; set; }
        public List<string> Allergens { get; set; } = new();
    }
}