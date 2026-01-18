using ClinicCare.Shared.DTOs.Admin;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<AdminResponseDto>> GetAllAsync();
        Task<AdminResponseDto?> GetByIdAsync(int id);
        Task UpdateAsync(int id, AdminUpdateDto dto);
        Task DeleteAsync(int id);
    }
}