using ClinicCare.Shared.DTOs.Enums;

namespace ClinicCare.Shared.DTOs.Appointment
{
    public class AppointmentCreateDto
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime Date { get; set; }
        public TimeSlotType TimeSlot { get; set; }
    }
}
