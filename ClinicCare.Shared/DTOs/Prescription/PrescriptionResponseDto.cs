using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.DTOs.Patient;

namespace ClinicCare.Shared.DTOs.Prescription
{
    public class PrescriptionResponseDto
    {
        public Guid Id { get; set; }
        public PatientMiniDto Patient { get; set; }
        public DoctorMiniDto Doctor { get; set; }
        public List<MedicationDto> Description { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

}
