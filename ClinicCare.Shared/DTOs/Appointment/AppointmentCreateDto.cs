using ClinicCare.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClinicCare.Shared.DTOs.Appointment
{
    public class AppointmentCreateDto
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        public Guid PaymentId { get; set; }

        [Required]
        [EnumDataType(typeof(TimeSlotType))]
        public TimeSlotType TimeSlot { get; set; }
    }
}
