using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.DTOs.Patient;
using ClinicCare.Shared.Enums;

namespace ClinicCare.Shared.DTOs.Appointment
{
    public class AppointmentResponseDto
    {
        public Guid Id { get; set; }
        public AppointmentStatus Status { get; set; }
        public Guid? PrescriptionId { get; set; }
        public DateOnly Date { get; set; }
        public TimeSlotType TimeSlot { get; set; }
        public DateTime CreatedAt { get; set; }
        public PatientMiniDto Patient { get; set; }
        public DoctorMiniDto Doctor { get; set; }
    }
}
