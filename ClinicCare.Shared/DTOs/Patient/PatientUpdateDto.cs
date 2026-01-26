using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ClinicCare.Shared.DTOs.Patient
{
    public class PatientUpdateDto
    {
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [MinLength(8)]
        [MaxLength(100)]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).*$",
            ErrorMessage = "Password must contain uppercase, lowercase, number and special character."
        )]
        public string? Password { get; set; }

        [Phone]
        [MaxLength(15)]
        public string? Phone { get; set; }

        [Phone]
        [MaxLength(15)]
        public string? EmergencyContact { get; set; }

        [MaxLength(5)]
        public string? BloodGroup { get; set; }

        [MaxLength(500)]
        public string? Allergies { get; set; }

        [Precision(5, 2)]
        public decimal? BodyWeight { get; set; }

        [Precision(5, 2)]
        public decimal? Height { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }
    }
}
