using ClinicCare.Shared.DTOs.Enums;

namespace ClinicCare.Shared.DTOs.Admin
{
    public class AdminResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public EmployeeRole Role { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfJoining { get; set; }
    }
}
