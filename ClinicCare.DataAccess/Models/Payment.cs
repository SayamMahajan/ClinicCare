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
        [Precision(10, 2)]
        public decimal Amount { get; set; }

        [Required]
        public Guid RecipientId { get; set; }

        [Required]
        public Guid SenderId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(RecipientId))]
        public Employee Recipient { get; set; }

        [ForeignKey(nameof(SenderId))]
        public Patient Sender { get; set; }
    }
}
