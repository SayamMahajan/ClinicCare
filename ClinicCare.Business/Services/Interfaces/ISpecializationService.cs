using ClinicCare.Shared.DTOs.Patient;
using ClinicCare.Shared.DTOs.Prescription;
using ClinicCare.Shared.DTOs.Specialization;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface ISpecializationService
    {
        Task<IEnumerable<SpecializationResponseDto>> GetAllAsync();
        Task<SpecializationResponseDto?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(SpecializationCreateDto dto);
        Task DeleteAsync(Guid id);
    }
}