using ClinicCare.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicCare.DataAccess.Models
{
    public class Payment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string TransactionId { get; set; }

        [Required]
        [Precision(10, 2)]
        public decimal Amount { get; set; }

        [Required]
        public PaymentType Type { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(PatientId))]
        public Patient Patient { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public Employee Doctor { get; set; }
    }
}
