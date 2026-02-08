using ClinicCare.DataAccess.Models;
using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.DTOs.Payment;
using ClinicCare.Shared.Enums;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaginatedResult<PaymentResponseDto>> GetAllAsync(PaymentSearchParams searchParams);
        Task<PaymentResponseDto?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(PaymentCreateDto dto, PaymentType paymentType = PaymentType.Paid);
        Task ProcessCancellationRefundAsync(Appointment appointment);
    }
}