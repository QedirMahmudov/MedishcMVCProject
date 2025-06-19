namespace MedishcMVCProject.Models
{
    public class Appointment : Base
    {
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string? Description { get; set; }
    }
}
