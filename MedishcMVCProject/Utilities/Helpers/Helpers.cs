namespace MedishcMVCProject.Utilities.Helpers
{
    public static class Helpers
    {
        public static string Capitalize(this string value)
        {
            return value.Substring(0, 1).ToUpper() + value.Substring(1).ToLower();
        }
    }
}
