using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ClinicCare.Shared.DTOs.Payment
{
    public class PaymentCreateDto
    {
        [Required]
        [Precision(10,2)]
        public decimal Amount { get; set; }

        [Required]
        public Guid RecipientId { get; set; }

        [Required]
        public Guid SenderId { get; set; }
    }
}
