using ClinicCare.DataAccess.Data;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClinicCare.DataAccess.Repositories
{
    public class PatientRepository : GenericRepository<Patient>, IPatientRepository
    {
        private readonly AppDbContext _context;

        public PatientRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Patient?> GetByEmailAsync(string email)
        {
            return await _context.Patients
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<int> GetTodayCountAsync()
        {
            return await _context.Patients.CountAsync(p => p.CreatedAt.Date == DateTime.Today);
        }

        public async Task<int> GetThisMonthCountAsync(DateOnly monthStart)
        {
            return await _context.Patients.CountAsync(p => p.CreatedAt.Date >= monthStart.ToDateTime(TimeOnly.MinValue));
        }

    }
}
