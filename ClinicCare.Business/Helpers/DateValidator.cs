using ClinicCare.Business.Exceptions;

namespace ClinicCare.Business.Helpers
{
    public static class DateValidator
    {
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

        public static void ValidateNotFuture(DateTime date, string fieldName)
        {
            if (date.Date > DateTime.UtcNow.Date)
                throw new BadRequestException($"{fieldName} cannot be in the future.");
        }

        public static void ValidateNotPast(DateTime date, string fieldName)
        {
            if (date.Date < DateTime.UtcNow.Date)
                throw new BadRequestException($"{fieldName} cannot be in the past.");
        }

        public static void ValidateAfter(DateTime later, DateTime earlier, string message)
        {
            if (later <= earlier)
                throw new BadRequestException(message);
        }
    }
}
