using MedishcMVCProject.Utilities;

namespace MedishcMVCProject.Models
{
    public class Patient : Base
    {
        public string Image { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public GenderEnum Gender { get; set; }
        public int Age { get; set; }
        public string MainDescription { get; set; }


        public int DiseaseId { get; set; }
        public Disease Disease { get; set; }
        public int BloodGroupId { get; set; }
        public BloodGroup? BloodGroup { get; set; }

        public List<PatientReport> Reports { get; set; } = new();
    }
}
