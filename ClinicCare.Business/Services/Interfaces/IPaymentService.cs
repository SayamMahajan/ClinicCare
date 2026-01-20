using ClinicCare.Shared.DTOs.Payment;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentResponseDto>> GetAllAsync();
        Task<PaymentResponseDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<PaymentResponseDto>> GetByRecipientAsync(Guid recipientId);
        Task<IEnumerable<PaymentResponseDto>> GetBySenderAsync(Guid senderId);
        Task<Guid> CreateAsync(PaymentCreateDto dto);
    }
}