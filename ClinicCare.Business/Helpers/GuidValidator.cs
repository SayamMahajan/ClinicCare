using ClinicCare.Business.Exceptions;

namespace ClinicCare.Business.Helpers
{
    public static class GuidValidator
    {
        public static void Validate(Guid id, string fieldName = "Id")
        {
            if (id == Guid.Empty)
                throw new BadRequestException($"{fieldName} is invalid.");
        }
    }
}
