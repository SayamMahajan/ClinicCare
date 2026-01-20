using ClinicCare.Shared.DTOs.Admin;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<AdminResponseDto>> GetAllAsync();
        Task<AdminResponseDto?> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, AdminUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}