using System.ComponentModel.DataAnnotations;

namespace ClinicCare.Shared.DTOs.Specialization
{
    public class SpecializationCreateDto
    {
        [Required]
        [MaxLength(50)]
        public string Type { get; set; }
    }
}
