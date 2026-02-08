using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.DTOs.Specialization;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface ISpecializationService
    {
        Task<PaginatedResult<SpecializationResponseDto>> GetAllAsync(PaginationParams pagination);
        Task<SpecializationResponseDto?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(SpecializationCreateDto dto);
        Task DeleteAsync(Guid id);
    }
}