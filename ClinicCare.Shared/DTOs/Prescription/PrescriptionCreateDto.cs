namespace ClinicCare.Shared.DTOs.Prescription
{
    public class PrescriptionCreateDto
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public List<MedicationDto> Description { get; set; } = new();
    }
}
