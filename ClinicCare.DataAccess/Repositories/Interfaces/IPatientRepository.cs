using ClinicCare.DataAccess.Models;

namespace ClinicCare.DataAccess.Repositories.Interfaces
{
    public interface IPatientRepository : IGenericRepository<Patient>
    {
        Task<Patient?> GetByEmailAsync(string email);

        Task<int> GetTodayCountAsync();

        Task<int> GetThisMonthCountAsync(DateOnly monthStart);
    }
}
