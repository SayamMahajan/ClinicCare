using ClinicCare.Shared.Enums;

namespace ClinicCare.Shared.DTOs.Employee
{
    public class EmployeeResponseDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public EmployeeRole Role { get; set; }
        public DateTime DateOfJoining { get; set; }

        // Doctor-only
        public Guid? SpecializationId { get; set; }
        public decimal? Fee { get; set; }
        public string? Phone { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime? FirstPracticeDate { get; set; }
    }
}
