namespace MedishcMVCProject.Models
{
    public class Clinic : Base
    {
        public string Image { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ClinicService> ClinicServices { get; set; }
    }
}
