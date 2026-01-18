using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicCare.DataAccess.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Precision(10, 2)]
        public decimal Amount { get; set; }

        [Required]
        public int RecipientId { get; set; }

        [Required]
        public int SenderId { get; set; } // PatientId

        // Navigation properties
        [ForeignKey(nameof(RecipientId))]
        public Employee Recipient { get; set; }

        [ForeignKey(nameof(SenderId))]
        public Patient Sender { get; set; }
    }
}
