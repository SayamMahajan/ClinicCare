namespace ClinicCare.Shared.DTOs.Prescription
{
    public class PrescriptionResponseDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public List<MedicationDto> Description { get; set; } = new();
    }
}
