namespace Refeitorio.ViewModels
{
    public class EditProductViewModel
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double Kcal { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbs { get; set; }
        public string Allergens { get; set; } = string.Empty;
        public int Stock { get; set; }

        public string CurrentImageFileName { get; set; } = "default.jpg";
        public IFormFile? NewImageFile { get; set; }
    }
}
