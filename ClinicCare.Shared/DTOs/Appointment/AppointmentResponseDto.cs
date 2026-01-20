using ClinicCare.Shared.Enums;

namespace ClinicCare.Shared.DTOs.Appointment
{
    public class AuthResponseDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public AppointmentStatus Status { get; set; }
        public DateTime Date { get; set; }
        public TimeSlotType TimeSlot { get; set; }
    }
}
