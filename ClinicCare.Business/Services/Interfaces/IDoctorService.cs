using ClinicCare.Shared.DTOs.Doctor;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorResponseDto>> GetAllAsync();
        Task<DoctorResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<DoctorResponseDto>> GetBySpecialistTypeAsync(string type);
        Task UpdateAsync(int id, DoctorUpdateDto dto);
        Task DeleteAsync(int id);
    }
}