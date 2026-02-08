using ClinicCare.DataAccess.Models;

namespace ClinicCare.DataAccess.Repositories.Interfaces
{
    public interface ISpecializationRepository : IGenericRepository<Specialization>
    {
        Task<Specialization?> GetByTypeAsync(string type);
    }
}
