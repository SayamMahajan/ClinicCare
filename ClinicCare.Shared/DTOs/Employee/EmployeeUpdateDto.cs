using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ClinicCare.Shared.DTOs.Employee
{
    public class EmployeeUpdateDto
    {
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [MinLength(8)]
        [MaxLength(100)]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).*$",
            ErrorMessage = "Password must contain uppercase, lowercase, number and special character."
        )]
        public string? Password { get; set; }

        [Precision(5, 2)]
        public decimal? Fee { get; set; }

        public Guid? SpecializationId { get; set; }

        [Phone]
        [MaxLength(15)]
        public string? Phone { get; set; }
    }
}
