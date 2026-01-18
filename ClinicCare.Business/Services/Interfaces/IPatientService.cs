using ClinicCare.Shared.DTOs.Patient;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientResponseDto>> GetAllAsync();
        Task<PatientResponseDto?> GetByIdAsync(int id);
        Task UpdateAsync(int id, PatientUpdateDto dto);
        Task DeleteAsync(int id);
    }
}