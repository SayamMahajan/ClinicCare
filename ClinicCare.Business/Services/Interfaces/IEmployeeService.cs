using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.Enums;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<EmployeeLoginResponseDto> LoginAsync(EmployeeLoginDto dto);
        Task<Guid> RegisterAsync(EmployeeRegisterDto dto);
        Task<PaginatedResult<EmployeeResponseDto>> GetAllAsync(EmployeeSearchParams searchParams);
        Task<AdminDashboardResponse> GetAdminDashboardAsync();
        Task<EmployeeResponseDto> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, EmployeeUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}