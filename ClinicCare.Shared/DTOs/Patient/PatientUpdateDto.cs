namespace ClinicCare.Shared.DTOs.Patient
{
    public class PatientUpdateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }    
        public string Phone { get; set; } 
        public string? Address { get; set; }
    }
}
