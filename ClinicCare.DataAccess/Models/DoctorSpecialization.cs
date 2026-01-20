using System.ComponentModel.DataAnnotations;

namespace ClinicCare.DataAccess.Models
{
    public class DoctorSpecialization
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Type { get; set; }

        public ICollection<DoctorDetail> DoctorDetails { get; set; } = [];
    }
}
