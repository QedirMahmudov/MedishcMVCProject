using MedishcMVCProject.Models;
using MedishcMVCProject.Utilities;
using System.ComponentModel.DataAnnotations;

namespace MedishcMVCProject.ViewModels
{
    public class CreateStaffVM
    {
        [MinLength(3, ErrorMessage = "Name must be minimum 3 characters long.")]
        [MaxLength(20, ErrorMessage = "Name should be less than 20 characters long")]
        public string Name { get; set; }
        [MinLength(5, ErrorMessage = "Surname must be minimum 5 characters long.")]
        [MaxLength(20, ErrorMessage = "Surname should be less than 20 characters long")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Choose Gender!")]
        public GenderEnum? Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public int? DesignationId { get; set; }
        public List<Designation>? Designations { get; set; }

        public IFormFile MainPhoto { get; set; }

    }
}
