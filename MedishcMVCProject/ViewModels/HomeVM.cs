using MedishcMVCProject.Models;

namespace MedishcMVCProject.ViewModels
{
    public class HomeVM
    {
        public List<Doctor> Doctors { get; set; }
        public List<Specialist> Specialists { get; set; }

        public List<Clinic> Clinics { get; set; }
        public List<ClinicService> ClinicServices { get; set; }


        public List<Blog> Blogs { get; set; }
        public List<Author> Authors { get; set; }
        public List<Category> Categories { get; set; }
    }
}
