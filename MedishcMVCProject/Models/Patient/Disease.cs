namespace MedishcMVCProject.Models
{
    public class Disease : Base
    {
        public string Name { get; set; }
        public List<Patient>? Patients { get; set; }

    }
}
