using System.ComponentModel.DataAnnotations;

namespace ClinicCare.Shared.DTOs.Prescription
{
    public class PrescriptionCreateDto
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        [MinLength(1)]
        public List<MedicationDto> Description { get; set; }
    }
}
