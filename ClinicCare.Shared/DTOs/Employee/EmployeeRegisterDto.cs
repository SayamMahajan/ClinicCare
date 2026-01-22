using ClinicCare.Shared.Enums;

namespace ClinicCare.Shared.DTOs.Employee
{
    public class EmployeeRegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime DateOfJoining { get; set; }
        public EmployeeRole Role { get; set; }

        // Doctor-only
        public DoctorRegisterDetailsDto? DoctorDetails { get; set; }
    }
    public class DoctorRegisterDetailsDto
    {
        public Guid SpecializationId { get; set; }
        public decimal Fee { get; set; }
        public DateTime DOB { get; set; }
        public string Phone { get; set; }
        public DateTime FirstPracticeDate { get; set; }
    }
}
