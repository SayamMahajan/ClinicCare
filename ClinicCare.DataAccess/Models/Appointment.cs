using ClinicCare.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClinicCare.DataAccess.Models
{
    public class Appointment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public AppointmentStatus Status { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public TimeSlotType TimeSlot { get; set; }

        [Required]
        public Guid PaymentId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Patient Patient { get; set; } = null!;
        public Employee Doctor { get; set; } = null!;
        public Payment Payment { get; set; } = null!;

        public Prescription? Prescription { get; set; }
    }
}