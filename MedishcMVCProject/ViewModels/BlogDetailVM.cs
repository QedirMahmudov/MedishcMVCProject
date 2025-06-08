using MedishcMVCProject.Models;

namespace MedishcMVCProject.ViewModels
{
    public class BlogDetailVM
    {
        public Blog Blog { get; set; }
        public Author Author { get; set; }
        public List<Category> Categories { get; set; }
    }
}
