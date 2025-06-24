using System.ComponentModel.DataAnnotations;

namespace MedishcMVCProject.ViewModels
{
    public class UpdateSpecialistVM
    {
        [MinLength(3, ErrorMessage = "Department Name must be minimum 5 characters long.")]
        [MaxLength(20, ErrorMessage = "Department Name should be less than 25 characters long")]
        public string DepartmentName { get; set; }
        [MinLength(5, ErrorMessage = "FullName must be minimum 8 characters long.")]
        [MaxLength(20, ErrorMessage = "Name should be less than 30 characters long")]
        public string HeadDoctorFullName { get; set; }
        public string DepartmentPhoneNumber { get; set; }
        public string DepartmentEmail { get; set; }
    }
}
