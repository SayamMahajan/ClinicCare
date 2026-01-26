using ClinicCare.DataAccess.Data;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ClinicCare.UnitTest.Repositories
{
    [TestClass]
    public class PaymentRepositoryTests
    {
        private readonly AppDbContext _context = null!;
        private readonly PaymentRepository _repo = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repo = new PaymentRepository(_context);
        }

    }
}