using ClinicCare.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClinicCare.Shared.DTOs.Patient
{
    public class PatientRegisterDto
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public DateOnly DOB { get; set; }

        [Required]
        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } 

        [Required]
        [Phone]
        [MaxLength(15)]
        public string Phone { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).*$",
            ErrorMessage = "Password must contain uppercase, lowercase, number and special character."
        )]
        public string Password { get; set; } 
    }
}
