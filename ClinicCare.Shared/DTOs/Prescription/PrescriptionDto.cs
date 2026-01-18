namespace ClinicCare.Shared.DTOs.Prescription
{
    public class PrescriptionResponseDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public List<MedicationDto> Description { get; set; } = new();
    }
}
