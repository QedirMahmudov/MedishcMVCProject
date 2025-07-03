using MedishcMVCProject.Models;
using System.ComponentModel.DataAnnotations;

namespace MedishcMVCProject.ViewModels.ProductVM
{
    public class CreateProductVM
    {
        [MinLength(3, ErrorMessage = "Product name must be minimum 3 characters long.")]
        [MaxLength(100, ErrorMessage = "Product name must be less than 100 characters.")]
        public string Name { get; set; }

        [MinLength(3, ErrorMessage = "SKU must be minimum 3 characters long.")]
        [MaxLength(30, ErrorMessage = "SKU must be less than 30 characters.")]
        public string SKU { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [MinLength(10, ErrorMessage = "Description must be minimum 10 characters long.")]
        [MaxLength(1000, ErrorMessage = "Description must be less than 1000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please select an image.")]
        public IFormFile MainPhoto { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        public int? CategoryId { get; set; }
        public List<ProductCategory>? ProductCategories { get; set; }

        [Required(ErrorMessage = "Please select a specialist.")]
        public int? SpecialistId { get; set; }
        public List<Specialist>? Specialists { get; set; }
    }
}
