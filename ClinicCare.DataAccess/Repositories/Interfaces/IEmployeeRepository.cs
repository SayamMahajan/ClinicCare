using ClinicCare.DataAccess.Models;
using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.DTOs.Pagination;

namespace ClinicCare.DataAccess.Repositories.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<PaginatedResult<Employee>> GetAllAsync(EmployeeSearchParams searchParams);

        Task<int> GetTotalDoctorsCountAsync();

        Task<Employee?> GetDoctorByIdAsync(Guid doctorId);

        Task<Employee?> GetByEmailAsync(string email);
    }
}
