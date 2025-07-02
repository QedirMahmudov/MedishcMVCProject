using System.ComponentModel.DataAnnotations;

namespace MedishcMVCProject.ViewModels
{
    public class LoginVM
    {
        [MaxLength(100)]
        public string UserNameOrEmail { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsPersistent { get; set; }
    }
}
