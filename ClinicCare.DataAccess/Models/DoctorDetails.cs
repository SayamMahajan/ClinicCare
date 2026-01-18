using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicCare.DataAccess.Models
{
    public class DoctorDetails
    {
        [Key]
        [ForeignKey(nameof(Employee))]
        public int DoctorId { get; set; }

        [Required]
        [Precision(10, 2)]
        public decimal Fee { get; set; }

        [Required]
        [MaxLength(100)]
        public string SpecialistType { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FirstPracticeDate { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        // Navigation property
        [ForeignKey(nameof(DoctorId))]
        public Employee Employee { get; set; }
    }
}
