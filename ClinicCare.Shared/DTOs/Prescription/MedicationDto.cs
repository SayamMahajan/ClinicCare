using System.ComponentModel.DataAnnotations;

namespace ClinicCare.Shared.DTOs.Prescription
{
    public class MedicationDto
    {
        [Required]
        [MaxLength(100)]
        public string Medicine { get; set; }

        [Required]
        [Range(1, 10)]
        public int Dosage { get; set; }

        [Required]
        [MaxLength(50)]
        public string Frequency { get; set; }

        [Required]
        [Range(1, 365)]
        public int Days { get; set; }

        [MaxLength(500)]
        public string Instructions { get; set; }
    }
}
