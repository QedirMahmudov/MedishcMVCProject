using MedishcMVCProject.Utilities;

namespace MedishcMVCProject.Models
{
    public class ContactInfo : Base
    {
        public ContactType ContactType { get; set; }
        public string? Value { get; set; }

        public OwnerType OwnerType { get; set; }
        public int OwnerId { get; set; }
    }
}
