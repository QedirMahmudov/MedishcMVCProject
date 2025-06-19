using MedishcMVCProject.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedishcMVCProject.ViewModels
{
    public class CreateAppointmentVM
    {
        public int SpecialistId { get; set; }
        public int DoctorId { get; set; }

        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }

        public string PatientName { get; set; }
        public string PatientSurname { get; set; }
        public GenderEnum Gender { get; set; }
        public int Age { get; set; }

        public int DiseaseId { get; set; }
        public int BloodGroupId { get; set; }
        public string? Problem { get; set; }
        public List<SelectListItem>? Specialists { get; set; }
        public List<SelectListItem>? Doctors { get; set; }
        public List<SelectListItem>? BloodGroups { get; set; }
        public List<SelectListItem>? Diseases { get; set; }
    }
}
