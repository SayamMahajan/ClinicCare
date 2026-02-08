using ClinicCare.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ClinicCare.DataAccess.Models
{
    public class Patient
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateOnly DOB { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        public string Password { get; set; }

        [Required]
        [Phone]
        [MaxLength(15)]
        public string Phone { get; set; } 

        // Optional fields
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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Appointment> Appointments { get; set; } = [];
        public ICollection<Payment> PaymentsSent { get; set; } = [];
        public ICollection<Prescription> Prescriptions { get; set; } = [];
    }
}
