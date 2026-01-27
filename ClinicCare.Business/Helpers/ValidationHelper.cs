using ClinicCare.Business.Exceptions;

namespace ClinicCare.Business.Helpers
{
    public static class ValidationHelper
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

        public static void ValidateAge(DateTime dob, int maxAge = 120)
        {
            var today = DateTime.UtcNow.Date;
            if (dob > today)
                throw new BadRequestException("Date of birth cannot be in the future.");

            var age = today.Year - dob.Year;
            if (dob > today.AddYears(-age))
                age--;

            if (age < 0 || age > maxAge)
                throw new BadRequestException($"Age must be between 0 and {maxAge} years.");
        }

        public static void DateNotInFuture(DateTime date, string fieldName)
        {
            if (date.Date > DateTime.UtcNow.Date)
                throw new BadRequestException($"{fieldName} cannot be in the future.");
        }

        public static void DateNotInPast(DateTime date, string fieldName)
        {
            if (date.Date < DateTime.UtcNow.Date)
                throw new BadRequestException($"{fieldName} cannot be in the past.");
        }
    }
}
