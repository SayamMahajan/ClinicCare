using ClinicCare.DataAccess.Data;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.DTOs.Prescription;
using Microsoft.EntityFrameworkCore;

namespace ClinicCare.DataAccess.Repositories
{
    public class PrescriptionRepository : GenericRepository<Prescription>, IPrescriptionRepository
    {
        private readonly AppDbContext _context;

        public PrescriptionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<Prescription>> GetAllAsync(
            PrescriptionSearchParams searchParams,
            Guid? patientId = null,
            Guid? doctorId = null)
        {
            IQueryable<Prescription> query = _context.Prescriptions
                .AsNoTracking()
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Doctor);

            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var searchTerm = searchParams.SearchTerm.Trim().ToLower();

                query = query.Where(p =>
                    EF.Functions.Like(p.Appointment.Patient.FirstName.ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like(p.Appointment.Patient.LastName.ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like(p.Appointment.Doctor.FirstName.ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like(p.Appointment.Doctor.LastName.ToLower(), $"%{searchTerm}%")
                );
            }

            if (patientId.HasValue)
                query = query.Where(p =>
                    p.Appointment.PatientId == patientId.Value);

            if (doctorId.HasValue)
                query = query.Where(p =>
                    p.Appointment.DoctorId == doctorId.Value);

            if (searchParams.AppointmentId.HasValue)
                query = query.Where(p =>
                    p.AppointmentId == searchParams.AppointmentId.Value);

            if (searchParams.StartDate.HasValue)
                query = query.Where(p =>
                    p.CreatedAt >= searchParams.StartDate.Value.ToDateTime(TimeOnly.MinValue));

            if (searchParams.EndDate.HasValue)
                query = query.Where(p =>
                    p.CreatedAt <= searchParams.EndDate.Value.ToDateTime(TimeOnly.MaxValue));

            return await GetPaginatedResultAsync(query, searchParams);
        }


        public new async Task<Prescription?> GetByIdAsync(Guid id)
        {
            return await _context.Prescriptions
                .AsNoTracking()
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Doctor)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        private async Task<PaginatedResult<Prescription>> GetPaginatedResultAsync(
            IQueryable<Prescription> query,
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

            return new PaginatedResult<Prescription>
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
