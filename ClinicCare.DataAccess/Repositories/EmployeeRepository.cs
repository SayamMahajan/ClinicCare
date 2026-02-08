using ClinicCare.DataAccess.Data;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClinicCare.DataAccess.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<Employee>> GetAllAsync(EmployeeSearchParams searchParams)
        {
            var query = _context.Employees
                .AsNoTracking()
                .Include(e => e.DoctorDetails)
                .Where(e => e.Role == searchParams.Role);

            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var searchTerm = searchParams.SearchTerm.Trim().ToLower();
                query = query.Where(e =>
                    EF.Functions.Like(e.FirstName.ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like(e.LastName.ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like(e.Phone.ToLower(), $"%{searchTerm}%"));
            }

            if (searchParams.Gender.HasValue)
                query = query.Where(e => e.Gender == searchParams.Gender);

            if (searchParams.SpecializationId.HasValue)
                query = query.Where(e => e.DoctorDetails != null && e.DoctorDetails.SpecializationId == searchParams.SpecializationId.Value);

            return await GetPaginatedResultAsync(query, searchParams);
        }

        public async Task<int> GetTotalDoctorsCountAsync()
        {
            return await _context.Employees.CountAsync(e => e.Role == EmployeeRole.Doctor);
        }


        public async Task<Employee?> GetDoctorByIdAsync(Guid doctorId)
        {
            return await _context.Employees
                .AsNoTracking()
                .Include(e => e.DoctorDetails!)
                    .ThenInclude(d => d.Specialization)
                .FirstOrDefaultAsync(e =>
                    e.Id == doctorId &&
                    e.Role == EmployeeRole.Doctor);
        }
            
        public async Task<Employee?> GetByEmailAsync(string email)
        {
            return await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Email == email);
        }

        private async Task<PaginatedResult<Employee>> GetPaginatedResultAsync(
            IQueryable<Employee> query,
            PaginationParams pageParams)
        {
            var pageNumber = Math.Max(1, pageParams.PageNumber);
            var pageSize = Math.Min(100, Math.Max(1, pageParams.PageSize));
            var skip = (pageNumber - 1) * pageSize;

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(e => e.CreatedAt)
                .ThenBy(e => e.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new PaginatedResult<Employee>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };
        }
    }
}
