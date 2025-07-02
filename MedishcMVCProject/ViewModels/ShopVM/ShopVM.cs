using MedishcMVCProject.Models;

namespace MedishcMVCProject.ViewModels
{
    public class ShopVM
    {
        public List<Product> Products { get; set; }
        public List<SpecialistCountVM> SpecialistCounts { get; set; }
        public int? SelectedSpecialistId { get; set; }
        public string? Search { get; set; }
        public List<Product>? RecentProducts { get; set; }
        public List<Tag>? Tags { get; set; }
        public int? SelectedTagId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? SortOrder { get; set; }

    }
}
