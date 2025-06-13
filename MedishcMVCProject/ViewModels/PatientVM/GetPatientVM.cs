using MedishcMVCProject.Models;

namespace MedishcMVCProject.ViewModels
{
    public class GetPatientVM
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? GenderName { get; set; }
        public int Age { get; set; }
        public string MainDescription { get; set; }
        public string DiseaseName { get; set; }
        public string BloodName { get; set; }
        public int PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<PatientReport> Reports { get; set; }
    }
}
