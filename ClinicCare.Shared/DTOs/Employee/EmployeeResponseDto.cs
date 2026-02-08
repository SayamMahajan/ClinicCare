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
        public DateTime CreatedAt { get; set; }
        public Gender Gender { get; set; }
        public string Phone { get; set; }
        public DateOnly DOB { get; set; }

        public Guid? SpecializationId { get; set; }
        public decimal? Fee { get; set; }
        public DateOnly? FirstPracticeDate { get; set; }
    }
}
