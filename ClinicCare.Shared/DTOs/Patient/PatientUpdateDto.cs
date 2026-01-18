namespace ClinicCare.Shared.DTOs.Patient
{
    public class PatientUpdateDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;    
        public string Phone { get; set; } = string.Empty;
        public string? Address { get; set; }
    }
}
