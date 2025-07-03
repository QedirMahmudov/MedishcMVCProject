using MedishcMVCProject.Models;
using System.ComponentModel.DataAnnotations;

namespace MedishcMVCProject.ViewModels.ProductVM
{
    public class UpdateProductVM
    {
        public int Id { get; set; }

        [Required, MinLength(3), MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required, MinLength(2), MaxLength(50)]
        public string SKU { get; set; }

        [Required]
        public int? CategoryId { get; set; }

        public List<ProductCategory>? ProductCategories { get; set; }

        [Required]
        public int? SpecialistId { get; set; }
        public List<Specialist>? Specialists { get; set; }

        public string? ImageUrl { get; set; }


        public IFormFile? MainPhoto { get; set; }

        [Required, MinLength(10), MaxLength(500)]
        public string? Description { get; set; }
    }
}
