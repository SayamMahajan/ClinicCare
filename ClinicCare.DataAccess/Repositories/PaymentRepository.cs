using ClinicCare.DataAccess.Data;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClinicCare.DataAccess.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetPaymentsForDoctorAsync(Guid doctorId)
        {
            return await _context.Payments
                .AsNoTracking()
                .Include(p => p.Recipient)
                .Include(p => p.Sender)
                .Where(p => p.RecipientId == doctorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsForPatientAsync(Guid patientId)
        {
            return await _context.Payments
                .AsNoTracking()
                .Include(p => p.Recipient)
                .Include(p => p.Sender)
                .Where(p => p.SenderId == patientId)
                .ToListAsync();
        }
    }
}
