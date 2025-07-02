using MedishcMVCProject.Models;

namespace MedishcMVCProject.ViewModels
{
    public class ProductDetailVM
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; }
    }
}
