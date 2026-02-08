using ClinicCare.DataAccess.Data;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Pagination;
using Microsoft.EntityFrameworkCore;

namespace ClinicCare.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<PaginatedResult<T>> GetAllAsync(PaginationParams pageParams)
        {
            var pageNumber = Math.Max(1, pageParams.PageNumber);
            var pageSize = Math.Min(100, Math.Max(1, pageParams.PageSize));
            var query = _dbSet.AsNoTracking();

            var totalCount = await query.CountAsync();

            query = query.OrderBy(e => EF.Property<Guid>(e, "Id"));

            var skip = (pageNumber - 1) * pageSize;
            var items = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new PaginatedResult<T>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };
        }

        public async Task<T?> GetByIdAsync(Guid id)
            => await _dbSet.FindAsync(id);

        
        public async Task InsertAsync(T entity)
            => await _dbSet.AddAsync(entity);
        
        public void Update(T entity)
            => _dbSet.Update(entity);

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
                _dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
