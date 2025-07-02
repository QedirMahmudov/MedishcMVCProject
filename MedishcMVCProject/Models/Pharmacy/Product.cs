using MedishcMVCProject.Models.Pharmacy;

namespace MedishcMVCProject.Models
{
    public class Product : Base
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }

        public int? SpecialistId { get; set; }
        public Specialist? Specialist { get; set; }

        public string? SKU { get; set; }

        public int? CategoryId { get; set; }
        public ProductCategory? Category { get; set; }

        public List<ProductTag> ProductTags { get; set; }
    }
}
