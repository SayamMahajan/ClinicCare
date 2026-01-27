using ClinicCare.DataAccess.Data;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories;
using ClinicCare.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClinicCare.UnitTest.Repositories
{
    [TestClass]
    public class GenericRepositoryTests
    {
        private AppDbContext _context = null!;
        private GenericRepository<Employee> _repo = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repo = new GenericRepository<Employee>(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private static Employee CreateEmployee(string email)
        {
            return new Employee
            {
                Id = Guid.NewGuid(),
                Role = EmployeeRole.Doctor,
                FirstName = "John",
                LastName = "Doe",
                Email = email,
                Password = "Password@123",
                DateOfJoining = DateTime.UtcNow
            };
        }

        [TestMethod]
        public async Task GetAllAsync_ValidRequest_ReturnsAllEntities()
        {
            _context.Employees.AddRange(
                CreateEmployee("a@test.com"),
                CreateEmployee("b@test.com")
            );
            await _context.SaveChangesAsync();

            var result = await _repo.GetAllAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }


        [TestMethod]
        public async Task GetByIdAsync_EntityExists_ReturnsEntity()
        {
            var employee = CreateEmployee("test@test.com");
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var result = await _repo.GetByIdAsync(employee.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(employee.Email, result.Email);
        }

        [TestMethod]
        public async Task GetByIdAsync_EntityDoesNotExist_ReturnsNull()
        {
            var result = await _repo.GetByIdAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task FindAsync_MatchingPredicate_ReturnsEntities()
        {
            _context.Employees.AddRange(
                CreateEmployee("doctor@test.com"),
                CreateEmployee("nurse@test.com")
            );
            await _context.SaveChangesAsync();

            var result = await _repo.FindAsync(e => e.Email.Contains("doctor"));

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("doctor@test.com", result.First().Email);
        }

        [TestMethod]
        public async Task FindAsync_NoMatch_ReturnsEmptyList()
        {
            _context.Employees.Add(CreateEmployee("test@test.com"));
            await _context.SaveChangesAsync();

            var result = await _repo.FindAsync(e => e.Email.Contains("xyz"));

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task InsertAsync_AddsEntity()
        {
            var employee = CreateEmployee("insert@test.com");

            await _repo.InsertAsync(employee);
            await _repo.SaveChangesAsync();

            Assert.AreEqual(1, _context.Employees.Count());
            Assert.AreEqual("insert@test.com", _context.Employees.First().Email);
        }

        [TestMethod]
        public async Task Update_ExistingEntity_UpdatesValues()
        {
            var employee = CreateEmployee("old@test.com");
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            employee.FirstName = "Updated";

            _repo.Update(employee);
            await _repo.SaveChangesAsync();

            var updated = await _context.Employees.FirstAsync();
            Assert.AreEqual("Updated", updated.FirstName);
        }

        [TestMethod]
        public async Task DeleteAsync_ExistingEntity_RemovesEntity()
        {
            var employee = CreateEmployee("delete@test.com");
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            await _repo.DeleteAsync(employee.Id);
            await _repo.SaveChangesAsync();

            Assert.AreEqual(0, _context.Employees.Count());
        }

        [TestMethod]
        public async Task DeleteAsync_EntityDoesNotExist_DoesNothing()
        {
            await _repo.DeleteAsync(Guid.NewGuid());
            await _repo.SaveChangesAsync();

            Assert.AreEqual(0, _context.Employees.Count());
        }
    }
}
