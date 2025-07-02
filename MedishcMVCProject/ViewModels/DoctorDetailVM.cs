using MedishcMVCProject.Models;

namespace MedishcMVCProject.ViewModels
{
    public class DoctorDetailVM
    {
        public Doctor Doctor { get; set; }
        public Degree Degree { get; set; }
        public Specialist Specialist { get; set; }
        public University University { get; set; }
        public List<WorkingHours> WorkingHours { get; set; }
        public List<PriceList> PriceLists { get; set; }

        public DoctorDetailVM()
        {
            WorkingHours = new List<WorkingHours>();
            PriceLists = new List<PriceList>();
        }
    }
}
