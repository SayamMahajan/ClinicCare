using ClinicCare.Shared.DTOs.Enums;

namespace ClinicCare.Shared.DTOs.Admin
{
    public class AdminUpdateDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
