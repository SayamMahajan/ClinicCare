using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.DTOs.Prescription;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IPrescriptionService
    {
        Task<PaginatedResult<PrescriptionResponseDto>> GetAllAsync(PrescriptionSearchParams searchParams);
        Task<PrescriptionResponseDto?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(PrescriptionCreateDto dto);
    }
}