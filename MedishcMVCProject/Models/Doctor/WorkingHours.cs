using MedishcMVCProject.Utilities;
using System.ComponentModel.DataAnnotations;

namespace MedishcMVCProject.Models
{
    public class WorkingHours : Base
    {
        public DayOfWeekEnum DayOfWeek { get; set; }
        [Required(ErrorMessage = "OpenTime is required")]
        public TimeSpan? OpenTime { get; set; }
        [Required(ErrorMessage = "CloseTime is required")]
        public TimeSpan? CloseTime { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
    }
}
