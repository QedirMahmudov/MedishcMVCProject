using MedishcMVCProject.Utilities;

namespace MedishcMVCProject.Models
{
    public class WorkingHours : Base
    {
        public DayOfWeekEnum DayOfWeek { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
    }
}
