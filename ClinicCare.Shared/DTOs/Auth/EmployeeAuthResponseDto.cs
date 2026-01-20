using ClinicCare.Shared.Enums;

namespace ClinicCare.Shared.DTOs.Auth
{
    public class EmployeeAuthResponseDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public string Email { get; set; } 
        public EmployeeRole Role { get; set; }
        public string Token { get; set; } 
    }
}
