using ClinicCare.Shared.DTOs.Pagination;

namespace ClinicCare.DataAccess.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<PaginatedResult<T>> GetAllAsync(PaginationParams pageParams);
        Task<T?> GetByIdAsync(Guid id);
        Task InsertAsync(T entity);
        void Update(T entity);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}
