using ClinicCare.Business.Exceptions;

namespace ClinicCare.Business.Helpers
{
    internal static class ValidationHelper
    {
        public static void NotNull(object? obj, string message)
        {
            if (obj is null)
                throw new BadRequestException(message);
        }

        public static void NotEmpty(string? value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BadRequestException(message);
        }

        public static void MustBeUnique(
            bool exists,
            string message)
        {
            if (exists)
                throw new ConflictException(message);
        }

        public static void GuidNotEmpty(Guid id, string fieldName = "Id")
        {
            if (id == Guid.Empty)
                throw new BadRequestException($"{fieldName} is invalid.");
        }

        public static void ValidateAge(DateOnly dob, int maxAge = 120)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

            if (dob > today)
                throw new BadRequestException("Date of birth cannot be in the future.");

            var age = today.Year - dob.Year;
            if (dob > today.AddYears(-age))
                age--;

            if (age < 0 || age > maxAge)
                throw new BadRequestException($"Age must be between 0 and {maxAge} years.");
        }

        public static void DateNotInFuture(DateOnly date, string fieldName)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

            if (date > today)
                throw new BadRequestException($"{fieldName} cannot be in the future.");
        }

        public static void DateNotInPast(DateOnly date, string fieldName)
        {
            if (date < DateOnly.FromDateTime(DateTime.UtcNow.Date))
                throw new BadRequestException($"{fieldName} cannot be in the past.");
        }

        public static void DateAtLeast24HoursAdvance(DateOnly date, string fieldName)
        {
            var minAllowedDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1).Date);
            if (date < minAllowedDate)
                throw new BadRequestException($"{fieldName} must be at least 24 hours in advance.");
        }
    }
}
