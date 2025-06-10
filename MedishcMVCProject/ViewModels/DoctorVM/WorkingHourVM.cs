using MedishcMVCProject.Utilities;
using System.ComponentModel.DataAnnotations;

namespace MedishcMVCProject.ViewModels
{
    public class WorkingHourVM
    {
        public DayOfWeekEnum DayOfWeek { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? OpenTime { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? CloseTime { get; set; }
    }
}
