namespace ClinicCare.Shared.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public Guid RecipientId { get; set; }
        public Guid SenderId { get; set; }
    }
}
