using ClinicCare.Shared.Enums;
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
        [EnumDataType(typeof(EmployeeRole))]
        public EmployeeRole Role { get; set; }

        public DateOnly DOB { get; set; }

        public Gender Gender { get; set; }

        [Phone]
        [MinLength(10)]
        [MaxLength(10)]
        public string Phone { get; set; }

        // Doctor-only
        public DoctorRegisterDetailsDto? DoctorDetails { get; set; }
    }
}
