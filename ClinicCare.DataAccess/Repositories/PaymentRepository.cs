using ClinicCare.DataAccess.Data;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.DTOs.Payment;
using ClinicCare.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClinicCare.DataAccess.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<Payment>> GetAllAsync(PaymentSearchParams searchParams, Guid? patientId = null, Guid? doctorId = null)
        {
            IQueryable<Payment> query = _context.Payments
                .AsNoTracking()
                .Include(p => p.Patient)
                .Include(p => p.Doctor);

            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var searchTerm = searchParams.SearchTerm.Trim().ToLower();
                query = query.Where(p =>
                    EF.Functions.Like(p.Patient.FirstName.ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like(p.Patient.LastName.ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like(p.Doctor.FirstName.ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like(p.Doctor.LastName.ToLower(), $"%{searchTerm}%"));
            }

            if (patientId.HasValue)
                query = query.Where(p => p.PatientId == patientId.Value);

            if (doctorId.HasValue)
                query = query.Where(p => p.DoctorId == doctorId.Value);

            if (searchParams.Type.HasValue)
                query = query.Where(p => p.Type == searchParams.Type.Value);

            if (searchParams.StartDate.HasValue)
                query = query.Where(p => p.CreatedAt >= searchParams.StartDate.Value.ToDateTime(TimeOnly.MinValue));

            if (searchParams.EndDate.HasValue)
                query = query.Where(p => p.CreatedAt <= searchParams.EndDate.Value.ToDateTime(TimeOnly.MaxValue));

            return await GetPaginatedResultAsync(query, searchParams);
        }

        public async Task<int> GetTodayCountAsync()
        {
            return await _context.Patients.CountAsync(p => p.CreatedAt.Date == DateTime.Today);
        }

        public async Task<int> GetThisMonthCountAsync(DateOnly monthStart)
        {
            return await _context.Patients.CountAsync(p => p.CreatedAt.Date >= monthStart.ToDateTime(TimeOnly.MinValue));
        }


        public new async Task<Payment?> GetByIdAsync(Guid id)
        {
            return await _context.Payments
                .AsNoTracking()
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        private async Task<PaginatedResult<Payment>> GetPaginatedResultAsync(
            IQueryable<Payment> query,
            PaginationParams pageParams)
        {
            var pageNumber = Math.Max(1, pageParams.PageNumber);
            var pageSize = Math.Min(100, Math.Max(1, pageParams.PageSize));
            var skip = (pageNumber - 1) * pageSize;

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(p => p.CreatedAt) 
                .ThenBy(p => p.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new PaginatedResult<Payment>
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
