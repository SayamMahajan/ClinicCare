using ClinicCare.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ClinicCare.Shared.DTOs.Employee
{
    public class EmployeeRegisterDto
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).*$",
            ErrorMessage = "Password must contain uppercase, lowercase, number and special character."
        )]
        public string Password { get; set; }

        [Required]
        public DateTime DateOfJoining { get; set; }

        [Required]
        [EnumDataType(typeof(EmployeeRole))]
        public EmployeeRole Role { get; set; }

        // Doctor-only
        public DoctorRegisterDetailsDto? DoctorDetails { get; set; }
    }
    public class DoctorRegisterDetailsDto
    {
        [Required]
        public Guid SpecializationId { get; set; }

        [Required]
        [Precision(10, 2)]
        public decimal Fee { get; set; }

        [Required]
        public DateTime DOB { get; set; }

        [Required]
        [Phone]
        [MaxLength(15)]
        public string Phone { get; set; }

        [Required]
        [Phone]
        [MaxLength(15)]
        public DateTime FirstPracticeDate { get; set; }
    }
}
