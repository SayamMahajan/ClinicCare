using ClinicCare.Shared.DTOs.Patient;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientResponseDto>> GetAllAsync();
        Task<PatientResponseDto?> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, PatientUpdateDto dto);
        Task DeleteAsync(Guid id);
        Task<Guid> RegisterPatientAsync(PatientRegisterDto dto);
        Task<PatientLoginResponseDto> LoginPatientAsync(PatientLoginDto dto);
    }
}