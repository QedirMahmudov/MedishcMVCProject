namespace MedishcMVCProject.Models
{
    public class University : Base
    {
        public string Name { get; set; }
        public List<Doctor>? Doctors { get; set; }
    }
}
