using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ClinicCare.Shared.DTOs.Employee
{
    public class DoctorRegisterDetailsDto
    {
        [Required]
        public Guid SpecializationId { get; set; }

        [Required]
        [Precision(10, 2)]
        public decimal Fee { get; set; }

        [Required]
        public DateTime DOB { get; set; }

        [Required]
        [Phone]
        [MaxLength(15)]
        public string Phone { get; set; }

        [Required]
        [Phone]
        [MaxLength(15)]
        public DateTime FirstPracticeDate { get; set; }
    }
}
