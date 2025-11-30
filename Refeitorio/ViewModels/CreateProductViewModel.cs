using System.ComponentModel.DataAnnotations;

namespace Refeitorio.ViewModels
{
    public class CreateProductViewModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Formato inválido")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Formato inválido")]
        public double Kcal { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Formato inválido")]
        public double Protein { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Formato inválido")]
        public double Fat { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Formato inválido")]
        public double Carbs { get; set; }

        [Required]
        public string Allergens { get; set; }

        [Required]
        public int Stock { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
