using MedishcMVCProject.Models;

namespace MedishcMVCProject.ViewModels.DoctorVM
{
    public class DoctorListVM
    {
        public List<Doctor> Doctors { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
