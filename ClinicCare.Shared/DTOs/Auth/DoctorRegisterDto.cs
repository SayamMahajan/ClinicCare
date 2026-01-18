using ClinicCare.Shared.DTOs.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClinicCare.Shared.DTOs.Auth
{
    public class DoctorRegisterDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public EmployeeRole Role { get { return EmployeeRole.Doctor; } }

        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).*$",
            ErrorMessage = "Password must contain uppercase, lowercase, number and special character."
        )]
        public string Password { get; set; } = string.Empty;

        public DateTime DateOfJoining { get; set; }

        public DateTime DOB { get; set; }

        public decimal Fee { get; set; }

        public string SpecialistType { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public DateTime FirstPracticeDate { get; set; }
    }
}
