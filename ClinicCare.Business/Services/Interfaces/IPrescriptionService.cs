using ClinicCare.Shared.DTOs.Prescription;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IPrescriptionService
    {
        Task<IEnumerable<PrescriptionResponseDto>> GetAllAsync();
        Task<PrescriptionResponseDto?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(PrescriptionCreateDto dto);
    }
}