using ClinicCare.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public DateTime Date { get; set; }

        [Required]
        public TimeSlotType TimeSlot { get; set; }

        public Guid? PaymentId { get; set; }

        public Guid? PrescriptionId { get; set; }

        //Navigation properties
        [ForeignKey(nameof(PatientId))]
        public Patient Patient { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public Employee Doctor { get; set; }

        [ForeignKey(nameof(PaymentId))]
        public Payment? Payment { get; set; }

        [ForeignKey(nameof(PrescriptionId))]
        public Prescription? Prescription { get; set; }
    }
}
