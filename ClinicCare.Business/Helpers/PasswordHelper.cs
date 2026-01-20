namespace ClinicCare.Business.Helpers
{
    public static class PasswordHelper
    {
        public static string Hash(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password.Trim());
        }

        public static bool Verify(string password, string hash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password.Trim(), hash);
        }
    }
}
