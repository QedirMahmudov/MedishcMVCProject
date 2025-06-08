using MedishcMVCProject.Models;

namespace MedishcMVCProject.ViewModels
{
    public class DoctorDetailVM
    {
        public Doctor Doctor { get; set; }
        public Degree Degree { get; set; }
        public Specialist Specialist { get; set; }
        public University University { get; set; }
        public List<OpeningHour> OpeningHours { get; set; }
        public List<PriceList> PriceLists { get; set; }
    }
}
