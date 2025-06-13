namespace MedishcMVCProject.Models
{
    public class Degree : Base
    {
        public string Name { get; set; }
        public List<Doctor>? Doctors { get; set; }
    }
}
