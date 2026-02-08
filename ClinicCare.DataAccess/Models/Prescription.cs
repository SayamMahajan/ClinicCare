using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicCare.DataAccess.Models
{
    public class Prescription
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(PatientId))]
        public Patient Patient { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public Employee Doctor { get; set; }
    }
}
