namespace ClinicCare.Business.Helpers
{
    public static class NormalizationHelper
    {
        public static string NormalizeKey(string value)
            => value.Trim().ToLowerInvariant();
    }
    
}
