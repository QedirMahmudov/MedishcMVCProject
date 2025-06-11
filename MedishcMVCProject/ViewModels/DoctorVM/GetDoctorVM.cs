using MedishcMVCProject.Models;

namespace MedishcMVCProject.ViewModels
{
    public class GetDoctorVM
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? DegreeName { get; set; }
        public int? Review { get; set; }
        public string MainDescription { get; set; }
        public string SpecialistName { get; set; }
        public List<PriceList>? PriceLists { get; set; }
        public List<WorkingHourVM> WorkingHours { get; set; } = new();
    }
}
