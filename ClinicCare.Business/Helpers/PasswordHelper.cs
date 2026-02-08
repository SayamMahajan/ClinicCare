using ClinicCare.Business.Exceptions;
using System.Text.RegularExpressions;

namespace ClinicCare.Business.Helpers
{
    internal static class PasswordHelper
    {
        private static readonly Regex StrongPassword =
            new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,100}$");

        public static string Hash(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        public static bool Verify(string password, string hash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
        }

        public static void Validate(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new BadRequestException("Password is required.");

            if (!StrongPassword.IsMatch(password))
                throw new BadRequestException(
                    "Password must contain uppercase, lowercase, number and special character.");
        }
    }
}
