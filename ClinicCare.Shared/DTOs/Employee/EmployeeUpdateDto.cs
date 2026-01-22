namespace ClinicCare.Shared.DTOs.Employee
{
    public class EmployeeUpdateDto
    {
        // Common
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Password { get; set; }

        // Doctor-only
        public decimal? Fee { get; set; }
        public Guid? SpecializationId { get; set; }
        public string? Phone { get; set; }
    }
}
