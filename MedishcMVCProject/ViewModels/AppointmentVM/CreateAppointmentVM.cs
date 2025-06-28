using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MedishcMVCProject.ViewModels
{
    public class CreateAppointmentVM
    {
        public string Email { get; set; }

        [Required]
        public int? SpecialistId { get; set; }
        [Required]
        public int? DoctorId { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; } = null!;
        public string Description { get; set; }
        [EmailAddress]
        public string? DoctorEmail { get; set; }
        public List<SelectListItem>? Specialists { get; set; }
        public List<SelectListItem>? Doctors { get; set; }
    }


}
