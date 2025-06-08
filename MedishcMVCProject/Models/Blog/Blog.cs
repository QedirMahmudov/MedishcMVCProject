namespace MedishcMVCProject.Models
{
    public class Blog : Base
    {
        public string Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<BlogCategory> BlogCategories { get; set; }
    }
}
