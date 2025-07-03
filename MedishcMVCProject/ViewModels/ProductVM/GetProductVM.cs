namespace MedishcMVCProject.ViewModels
{
    public class GetProductVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string SpecialistName { get; set; }
        public int? SpecialistId { get; set; }
        public string Image { get; set; }
    }
}
