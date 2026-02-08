using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClinicCare.Shared.DTOs.Employee
{
    public class EmployeeSearchParams : PaginationParams
    {
        public string? SearchTerm { get; set; }
        [Required]
        public EmployeeRole Role { get; set; } = EmployeeRole.Doctor;
        public Gender? Gender { get; set; }
        public Guid? SpecializationId { get; set; }
    }

}
