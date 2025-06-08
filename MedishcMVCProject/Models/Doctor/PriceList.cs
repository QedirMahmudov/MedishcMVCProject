namespace MedishcMVCProject.Models
{
    public class PriceList : Base
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
    }
}
