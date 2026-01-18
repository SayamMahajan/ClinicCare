using ClinicCare.Shared.DTOs.Enums;

namespace ClinicCare.Shared.DTOs.Auth
{
    public class EmployeeAuthResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public EmployeeRole Role { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
