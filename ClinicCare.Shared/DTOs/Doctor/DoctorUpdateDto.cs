namespace ClinicCare.Shared.DTOs.Doctor
{
    public class DoctorUpdateDto 
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public decimal Fee { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string SpecialistType { get; set; } = string.Empty;
    }
}
