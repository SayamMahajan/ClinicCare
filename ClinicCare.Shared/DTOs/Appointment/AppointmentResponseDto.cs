using ClinicCare.Shared.DTOs.Enums;

namespace ClinicCare.Shared.DTOs.Appointment
{
    public class AppointmentResponseDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public AppointmentStatus Status { get; set; }
        public DateTime Date { get; set; }
        public TimeSlotType TimeSlot { get; set; }
    }
}
