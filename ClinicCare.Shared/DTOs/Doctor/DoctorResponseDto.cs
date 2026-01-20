using ClinicCare.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClinicCare.Shared.DTOs.Doctor
{
    public class DoctorResponseDto 
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public string Email { get; set; } 
        public EmployeeRole Role { get; set; }
        public decimal Fee { get; set; }
        public Guid SpecializationId { get; set; }
        public DateTime DOB { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string Phone { get; set; } 
        public DateTime FirstPracticeDate { get; set; }
    }
}
