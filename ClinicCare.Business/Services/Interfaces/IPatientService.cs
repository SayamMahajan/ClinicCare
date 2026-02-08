using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.DTOs.Patient;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IPatientService
    {
        Task<PatientLoginResponseDto> LoginPatientAsync(PatientLoginDto dto);
        Task<Guid> RegisterPatientAsync(PatientRegisterDto dto);
        Task<PaginatedResult<PatientResponseDto>> GetAllAsync(PaginationParams pagination);
        Task<PatientResponseDto?> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, PatientUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}