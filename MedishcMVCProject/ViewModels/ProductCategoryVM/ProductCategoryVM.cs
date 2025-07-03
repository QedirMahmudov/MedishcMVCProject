using System.ComponentModel.DataAnnotations;

namespace MedishcMVCProject.ViewModels
{
    public class ProductCategoryVM
    {
        public int Id { get; set; }
        [MinLength(3, ErrorMessage = "Product category name must be minimum 3 characters long.")]
        [MaxLength(100, ErrorMessage = "Product category name must be less than 100 characters.")]
        public string CategoryName { get; set; }
    }
}
