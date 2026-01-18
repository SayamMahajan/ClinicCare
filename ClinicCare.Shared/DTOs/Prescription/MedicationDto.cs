namespace ClinicCare.Shared.DTOs.Prescription
{
    public class MedicationDto
    {
        public string Medicine { get; set; } = string.Empty;
        public int Dosage { get; set; }
        public string Frequency { get; set; } = string.Empty;
        public int Days { get; set; }
        public string Instructions { get; set; } = string.Empty;
    }
}
