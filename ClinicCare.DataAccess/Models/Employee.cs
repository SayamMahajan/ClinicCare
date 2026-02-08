using ClinicCare.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClinicCare.DataAccess.Models
{
    public class Employee
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public EmployeeRole Role { get; set; }

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
        public string Password { get; set; }

        [Required]
        public DateOnly DOB { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [Phone]
        [MaxLength(15)]
        public string Phone { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DoctorDetail? DoctorDetails { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = [];
        public ICollection<Payment> PaymentsReceived { get; set; } = [];
        public ICollection<Prescription> PrescriptionsWritten { get; set; } = [];
    }
}
