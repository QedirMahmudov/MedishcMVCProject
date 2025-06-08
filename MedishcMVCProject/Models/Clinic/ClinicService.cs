namespace MedishcMVCProject.Models
{
    public class ClinicService : Base
    {
        public string Name { get; set; }

        public int ClinicId { get; set; }
        public Clinic Clinic { get; set; }
    }
}
