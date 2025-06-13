using MedishcMVCProject.Utilities;

namespace MedishcMVCProject.Models
{
    public class Doctor : Base
    {
        public string Image { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public decimal? ZodocRating { get; set; }
        public int? ReviewCount { get; set; }

        public string MainDescription { get; set; }
        public string? AdditionalDescription { get; set; }

        public int Age { get; set; }
        public GenderEnum Gender { get; set; }

        public int SpecialistId { get; set; }
        public Specialist Specialist { get; set; }

        public int? DegreeId { get; set; }
        public Degree? Degree { get; set; }

        public int? UniversityId { get; set; }
        public University? University { get; set; }

        public List<PriceList>? PriceLists { get; set; }
        public List<WorkingHours>? OpeningHours { get; set; }

    }
}
