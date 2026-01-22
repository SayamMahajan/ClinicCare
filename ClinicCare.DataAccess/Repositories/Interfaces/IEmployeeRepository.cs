using ClinicCare.DataAccess.Models;
using ClinicCare.Shared.Enums;

namespace ClinicCare.DataAccess.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<IEnumerable<Employee>> GetDoctorsAsync(Guid? specializationId);
        Task<Employee?> GetDoctorWithDetailsAsync(Guid doctorId);
    }
}
