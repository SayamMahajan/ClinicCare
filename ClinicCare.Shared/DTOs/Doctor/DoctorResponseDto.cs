using ClinicCare.Shared.DTOs.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClinicCare.Shared.DTOs.Doctor
{
    public class DoctorResponseDto 
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public EmployeeRole Role { get; set; }
        public decimal Fee { get; set; }
        public string SpecialistType { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string Phone { get; set; } = string.Empty;
        public DateTime FirstPracticeDate { get; set; }
    }
}
