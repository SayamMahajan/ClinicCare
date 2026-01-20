using ClinicCare.Shared.DTOs.Doctor;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorResponseDto>> GetAllAsync(Guid? specializationId = null);
        Task<DoctorResponseDto?> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, DoctorUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}