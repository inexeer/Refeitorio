namespace Refeitorio.ViewModels
{
    public class CreateProductViewModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public double Kcal { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbs { get; set; }
        public string Allergens { get; set; }
        public int Stock { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
