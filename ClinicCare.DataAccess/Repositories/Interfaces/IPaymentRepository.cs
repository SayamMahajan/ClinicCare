using ClinicCare.DataAccess.Models;
using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.DTOs.Payment;
using ClinicCare.Shared.Enums;

namespace ClinicCare.DataAccess.Repositories.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<PaginatedResult<Payment>> GetAllAsync(PaymentSearchParams searchParams, Guid? patientId = null, Guid? doctorId = null);

    }
}
