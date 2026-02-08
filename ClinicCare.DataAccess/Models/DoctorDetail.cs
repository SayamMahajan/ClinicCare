using ClinicCare.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicCare.DataAccess.Models
{
    public class DoctorDetail
    {
        [Key]
        public Guid DoctorId { get; set; }

        [Required]
        [Precision(10, 2)]
        public decimal Fee { get; set; }

        [Required]
        public Guid SpecializationId { get; set; }

        [Required]
        public DateOnly FirstPracticeDate { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public Employee Employee { get; set; }

        [ForeignKey(nameof(SpecializationId))]
        public Specialization Specialization { get; set; }
    }
}
