using ClinicCare.Shared.DTOs.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ClinicCare.DataAccess.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Required]
        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty; 

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        // Optional fields
        [Phone]
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

        // Navigation properties
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Payment> PaymentsSent { get; set; } = new List<Payment>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
