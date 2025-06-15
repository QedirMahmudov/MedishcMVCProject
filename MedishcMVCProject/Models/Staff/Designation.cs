namespace MedishcMVCProject.Models
{
    public class Designation : Base
    {
        public string Name { get; set; }
        public List<Staff>? Staffs { get; set; }

    }
}
