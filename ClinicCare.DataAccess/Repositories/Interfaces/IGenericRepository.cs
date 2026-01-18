using System.Linq.Expressions;

namespace ClinicCare.DataAccess.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task InsertAsync(T entity);
        void Update(T entity);
        Task Delete(object id);
        Task SaveChangesAsync();
    }
}
