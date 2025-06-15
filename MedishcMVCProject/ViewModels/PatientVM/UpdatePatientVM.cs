using MedishcMVCProject.Models;
using MedishcMVCProject.Utilities;
using System.ComponentModel.DataAnnotations;

namespace MedishcMVCProject.ViewModels
{
    public class UpdatePatientVM
    {
        public string Image { get; set; }

        [MinLength(3, ErrorMessage = "Name must be minimum 3 characters long.")]
        [MaxLength(20, ErrorMessage = "Name should be less than 20 characters long")]
        public string Name { get; set; }

        [MinLength(5, ErrorMessage = "Surname must be minimum 5 characters long.")]
        [MaxLength(20, ErrorMessage = "Surname should be less than 20 characters long")]
        public string Surname { get; set; }

        [Range(1, 100, ErrorMessage = "Age must be between 1 and 100")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Choose Gender!")]
        public GenderEnum? Gender { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string MainDescription { get; set; }

        [Required]
        public int? BloodGroupId { get; set; }
        public List<BloodGroup>? BloodGroups { get; set; }

        [Required]
        public int? DiseaseId { get; set; }
        public List<Disease>? Diseases { get; set; }

        public IFormFile? MainPhoto { get; set; }

        public List<IFormFile>? ReportFiles { get; set; }

        public List<PatientReportVM>? ExistingReports { get; set; } = new();

        public List<int>? RemovedReportIds { get; set; } = new();

        public string? SocialMediaFacebook { get; set; }
        public string? SocialMediaTwitter { get; set; }
    }
}
