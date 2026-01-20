using ClinicCare.Shared.DTOs.Doctor;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorResponseDto>> GetAllAsync();
        Task<DoctorResponseDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<DoctorResponseDto>> GetBySpecializationIdAsync(Guid id);
        Task UpdateAsync(Guid id, DoctorUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}