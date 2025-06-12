using MedishcMVCProject.DAL;
using MedishcMVCProject.Models;

namespace MedishcMVCProject.Utilities.Helpers
{
    public static class Helpers
    {
        //Capitalize
        public static string Capitalize(this string value)
        {
            return value.Substring(0, 1).ToUpper() + value.Substring(1).ToLower();
        }

        public static bool HasDigit(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return !input.Any(char.IsDigit);
        }
        //search

        public static List<T> FilterByText<T>(List<T> items, Func<T, string> selector, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return items;

            searchText = searchText.ToLower();
            return items
                .Where(item => selector(item)?.ToLower().Contains(searchText) == true)
                .ToList();
        }


        public static void AddContactInfos(
        AppDbContext context,
        OwnerType ownerType,
        int ownerId,
        List<(ContactType ContactType, string? Value)> contactList)
        {
            List<ContactInfo>? validContacts = contactList
                .Where(c => !string.IsNullOrWhiteSpace(c.Value))
                .Select(c => new ContactInfo
                {
                    ContactType = c.ContactType,
                    Value = c.Value,
                    OwnerType = ownerType,
                    OwnerId = ownerId
                })
                .ToList();

            if (validContacts.Any())
            {
                context.ContactInfos.AddRange(validContacts);
            }
        }


    }
}
