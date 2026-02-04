using ClinicCare.DataAccess.Data;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClinicCare.DataAccess.Repositories
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly AppDbContext _context;

        public PrescriptionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Prescription?> GetByIdAsync(Guid id)
        {
            return await _context.Prescriptions
                .AsNoTracking()
                .Include(p => p.Doctor)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsForDoctorAsync(Guid doctorId)
        {
            return await _context.Prescriptions
                .AsNoTracking()
                .Include(p => p.Doctor)
                .Include(p => p.Patient)
                .Where(p => p.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsForPatientAsync(Guid patientId)
        {
            return await _context.Prescriptions
                .AsNoTracking()
                .Include(p => p.Doctor)
                .Include(p => p.Patient)
                .Where(p => p.PatientId == patientId)
                .ToListAsync();
        }
    }
}
