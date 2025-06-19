using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedishcMVCProject.ViewModels
{
    public class CreateAppointmentVM
    {
        public string Email { get; set; }
        public int SpecialistId { get; set; }
        public int DoctorId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string? Description { get; set; }

        public List<SelectListItem>? Specialists { get; set; }
        public List<SelectListItem>? Doctors { get; set; }
    }


}
