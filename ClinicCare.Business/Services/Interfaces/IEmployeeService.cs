using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.Enums;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<EmployeeLoginResponseDto> LoginAsync(EmployeeLoginDto dto);
        Task<Guid> RegisterAsync(EmployeeRegisterDto dto);
        Task<IEnumerable<EmployeeResponseDto>> GetAllAsync(EmployeeRole? role);
        Task<IEnumerable<EmployeeResponseDto>> GetAllDoctorsAsync(Guid? specializationId);
        Task<EmployeeResponseDto> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, EmployeeUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}