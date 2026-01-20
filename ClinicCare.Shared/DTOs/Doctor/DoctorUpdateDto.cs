namespace ClinicCare.Shared.DTOs.Doctor
{
    public class DoctorUpdateDto 
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public decimal Fee { get; set; }
        public string Phone { get; set; }
        public Guid SpecializationId { get; set; }
    }
}
