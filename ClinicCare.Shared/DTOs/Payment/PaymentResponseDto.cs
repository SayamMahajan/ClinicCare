namespace ClinicCare.Shared.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int RecipientId { get; set; }
        public int SenderId { get; set; }
    }
}
