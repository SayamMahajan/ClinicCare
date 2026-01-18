using ClinicCare.Shared.DTOs.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClinicCare.DataAccess.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EnumDataType(typeof(EmployeeRole))]
        public EmployeeRole Role { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfJoining { get; set; }

        [Required]
        [MinLength(8), MaxLength(100)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).*$",
            ErrorMessage = "Password must contain uppercase, lowercase, number and special character."
        )]
        public string Password { get; set; } = string.Empty;

        // Navigation properties
        public DoctorDetails? DoctorDetails { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Payment> PaymentsReceived { get; set; } = new List<Payment>();
        public ICollection<Prescription> PrescriptionsWritten { get; set; } = new List<Prescription>();
    }
}
