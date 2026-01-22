using ClinicCare.DataAccess.Data;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClinicCare.DataAccess.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Appointment>> GetAllWithDetailsAsync()
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Payment)
                .Include(a => a.Prescription)
                .ToListAsync();
        }

        public async Task<Appointment?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Payment)
                .Include(a => a.Prescription)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Where(a => a.PatientId == patientId)
                .ToListAsync();
        }
    }
}
