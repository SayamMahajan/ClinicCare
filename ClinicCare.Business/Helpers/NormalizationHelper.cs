namespace ClinicCare.Business.Helpers
{
    internal static class NormalizationHelper
    {
        public static string NormalizeKey(string value)
            => value.Trim().ToLowerInvariant();
    }
    
}
