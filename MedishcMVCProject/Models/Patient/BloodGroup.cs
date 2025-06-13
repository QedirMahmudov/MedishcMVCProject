namespace MedishcMVCProject.Models
{
    public class BloodGroup : Base
    {
        public string Name { get; set; }
        public List<Patient>? Patients { get; set; }
    }
}
