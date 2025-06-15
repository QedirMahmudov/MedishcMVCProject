using MedishcMVCProject.Utilities;

namespace MedishcMVCProject.Models
{
    public class Staff : Base
    {
        public string Image { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int DesignationId { get; set; }
        public GenderEnum Gender { get; set; }
        public Designation Designation { get; set; }

    }
}
