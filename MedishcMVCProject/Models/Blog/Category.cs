namespace MedishcMVCProject.Models
{
    public class Category : Base
    {
        public string Name { get; set; }
        public List<BlogCategory>? Blogs { get; set; }
    }
}
