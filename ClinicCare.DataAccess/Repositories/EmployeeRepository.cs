using ClinicCare.DataAccess.Data;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClinicCare.DataAccess.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Include(e => e.DoctorDetails)
                .AsNoTracking()
                .ToListAsync();
        }
            
        public async Task<IEnumerable<Employee>> GetDoctorsAsync(Guid? specializationId)
        {
            var query = _context.Employees
                .Include(e => e.DoctorDetails)
                .Where(e => e.Role == EmployeeRole.Doctor);

            if (specializationId != null)
                query = query.Where(e =>
                    e.DoctorDetails!.SpecializationId == specializationId);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<Employee?> GetDoctorWithDetailsAsync(Guid doctorId)
        {
            return await _context.Employees
                .AsNoTracking()
                .Include(e => e.DoctorDetails!)
                    .ThenInclude(d => d.DoctorSpecialization)
                .FirstOrDefaultAsync(e =>
                    e.Id == doctorId &&
                    e.Role == EmployeeRole.Doctor);
        }
    }
}
