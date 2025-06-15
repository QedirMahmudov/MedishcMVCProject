using MedishcMVCProject.Models;
using MedishcMVCProject.Utilities;
using System.ComponentModel.DataAnnotations;

namespace MedishcMVCProject.ViewModels
{
    public class UpdateDoctorVM
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
        public GenderEnum Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public int? SpecialistId { get; set; }
        public List<Specialist>? Specialists { get; set; }
        public List<WorkingHourVM>? WorkingHours { get; set; } = new();
        public IFormFile? MainPhoto { get; set; }


        [Range(1, 5, ErrorMessage = "ZodocRating must be between 1 and 5")]
        public decimal? ZodocRating { get; set; }
        public int? ReviewCount { get; set; }
        public string? SocialMediaFacebook { get; set; }
        public string? SocialMediaTwitter { get; set; }
        public string MainDescription { get; set; }
        public string? AdditionalDescription { get; set; }
    }
}
