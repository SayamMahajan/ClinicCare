using ClinicCare.DataAccess.Data;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClinicCare.DataAccess.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<PaginatedResult<Appointment>> GetAllAsync(AppointmentSearchParams searchParams, Guid? patientId, Guid? doctorId)
        {
            IQueryable<Appointment> query = _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Payment)
                .Include(a => a.Prescription);

            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var searchTerm = searchParams.SearchTerm.Trim().ToLower();
                query = query.Where(a =>
                    EF.Functions.Like(a.Patient.FirstName.ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like(a.Patient.LastName.ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like(a.Doctor.FirstName.ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like(a.Doctor.LastName.ToLower(), $"%{searchTerm}%"));
            }

            if (patientId.HasValue)
                query = query.Where(a => a.PatientId == patientId.Value);

            if (doctorId.HasValue)
                query = query.Where(a => a.DoctorId == doctorId.Value);

            if (searchParams.Status.HasValue) 
                query = query.Where(a => a.Status == searchParams.Status.Value);

            if (searchParams.PaymentId.HasValue)
                query = query.Where(a => a.PaymentId == searchParams.PaymentId.Value); 

            if (searchParams.StartDate.HasValue)
                query = query.Where(a => a.Date >= searchParams.StartDate.Value);

            if (searchParams.EndDate.HasValue) 
                query = query.Where(a => a.Date <= searchParams.EndDate.Value);

            return await GetPaginatedResultAsync(query, searchParams);
        }

        public new async Task<Appointment?> GetByIdAsync(Guid id)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Payment)
                .Include(a => a.Prescription)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<int> GetTodayCountAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _context.Appointments.CountAsync(a => a.Date == today);
        }

        public async Task<int> GetThisMonthCountAsync(DateOnly monthStart)
        {
            return await _context.Appointments.CountAsync(a => a.Date >= monthStart);
        }


        public async Task<IEnumerable<Appointment>> GetPatientAppointmentsForConflictCheckAsync(
            Guid patientId, DateOnly date, TimeSlotType timeSlot)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId
                         && a.Date == date
                         && a.TimeSlot == timeSlot
                         && a.Status != AppointmentStatus.Cancelled)
                .ToListAsync();
        }

        private async Task<PaginatedResult<Appointment>> GetPaginatedResultAsync(
            IQueryable<Appointment> query,
            PaginationParams pageParams)
        {
            var pageNumber = Math.Max(1, pageParams.PageNumber);
            var pageSize = Math.Min(100, Math.Max(1, pageParams.PageSize));
            var skip = (pageNumber - 1) * pageSize;

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(a => a.Date)
                .ThenBy(a => a.Id) 
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new PaginatedResult<Appointment>
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
