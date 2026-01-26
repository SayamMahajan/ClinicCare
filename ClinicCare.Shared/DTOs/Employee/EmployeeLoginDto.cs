using System.ComponentModel.DataAnnotations;

namespace ClinicCare.Shared.DTOs.Employee
{
    public class EmployeeLoginDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        public string Password { get; set; }
    }
}
