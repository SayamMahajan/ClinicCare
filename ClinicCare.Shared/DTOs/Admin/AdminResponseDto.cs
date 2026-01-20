using ClinicCare.Shared.Enums;

namespace ClinicCare.Shared.DTOs.Admin
{
    public class AdminResponseDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public EmployeeRole Role { get; set; }
        public string Email { get; set; }
        public DateTime DateOfJoining { get; set; }
    }
}
