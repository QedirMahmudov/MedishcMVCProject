namespace MedishcMVCProject.Models
{
    public class PatientReport : Base
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
