namespace MedishcMVCProject.Models
{
    public class Specialist : Base
    {
        public string Name { get; set; }
        public List<Doctor>? Doctors { get; set; }
    }
}
