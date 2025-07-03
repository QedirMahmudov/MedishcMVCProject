using System.ComponentModel.DataAnnotations;

namespace MedishcMVCProject.ViewModels
{
    public class CreateSpecialistVM
    {
        [MinLength(3, ErrorMessage = "Department Name must be minimum 5 characters long.")]
        [MaxLength(20, ErrorMessage = "Department Name should be less than 25 characters long")]
        public string DepartmentName { get; set; }
        [MinLength(5, ErrorMessage = "FullName must be minimum 8 characters long.")]
        [MaxLength(20, ErrorMessage = "Name should be less than 30 characters long")]
        public string HeadDoctorFullName { get; set; }
        [RegularExpression(@"^(?:\+994|0)(50|51|55|70|77|99)[0-9]{7}$", ErrorMessage = "Please enter a valid phone number.")]
        public string DepartmentPhoneNumber { get; set; }
        [RegularExpression(@"^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$", ErrorMessage = "Please enter a valid email in lowercase.")]
        public string DepartmentEmail { get; set; }
    }
}
