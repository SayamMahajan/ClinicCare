using ClinicCare.Shared.DTOs.Payment;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentResponseDto>> GetAllAsync();
        Task<PaymentResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<PaymentResponseDto>> GetByRecipientAsync(int recipientId);
        Task<IEnumerable<PaymentResponseDto>> GetBySenderAsync(int senderId);
        Task<int> CreateAsync(PaymentCreateDto dto);
        //Task DeleteAsync(int id);
    }
}