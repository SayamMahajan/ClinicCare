using ClinicCare.DataAccess.Data;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClinicCare.DataAccess.Repositories
{
    public class SpecializationRepository : GenericRepository<Specialization>, ISpecializationRepository
    {
        private readonly AppDbContext _context;

        public SpecializationRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Specialization?> GetByTypeAsync(string type)
        {
            return await _context.Specializations
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Type == type);
        }
    }
}
