using ClinicCare.DataAccess.Data;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories;
using ClinicCare.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClinicCare.UnitTest.Repositories
{
    [TestClass]
    public class EmployeeRepositoryTests
    {
        private AppDbContext _context = null!;
        private EmployeeRepository _repo = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repo = new EmployeeRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private static Employee CreateDoctor(Guid? specializationId = null)
        {
            var newId = Guid.NewGuid();
            return new Employee
            {
                Id = newId,
                Role = EmployeeRole.Doctor,
                FirstName = "Doctor",
                LastName = "Test",
                Email = Guid.NewGuid() + "@doctor.com",
                Password = "Password@123",
                DateOfJoining = DateTime.UtcNow,
                DoctorDetails = new DoctorDetail
                {
                    DoctorId = newId,
                    SpecializationId = specializationId ?? Guid.NewGuid(),
                    DOB = DateTime.UtcNow.AddYears(-30),
                    FirstPracticeDate = DateTime.UtcNow.AddYears(-8),
                    Phone = "9999999999",
                }
            };
        }

        private static Employee CreateNonDoctor()
        {
            return new Employee
            {
                Id = Guid.NewGuid(),
                Role = EmployeeRole.Admin,
                FirstName = "Staff",
                LastName = "Test",
                Email = Guid.NewGuid() + "@staff.com",
                Password = "Password@123",
                DateOfJoining = DateTime.UtcNow
            };
        }

        [TestMethod]
        public async Task GetAllAsync_ReturnsAllEmployees_WithDoctorDetails()
        {
            _context.Employees.AddRange(
                CreateDoctor(),
                CreateNonDoctor()
            );
            await _context.SaveChangesAsync();

            var result = await _repo.GetAllAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(e => e.Role == EmployeeRole.Doctor));
        }

        [TestMethod]
        public async Task GetDoctorsAsync_NoSpecialization_ReturnsAllDoctors()
        {
            _context.Employees.AddRange(
                CreateDoctor(),
                CreateDoctor(),
                CreateNonDoctor()
            );
            await _context.SaveChangesAsync();

            var result = await _repo.GetDoctorsAsync(null);

            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(e => e.Role == EmployeeRole.Doctor));
        }

        [TestMethod]
        public async Task GetDoctorsAsync_WithSpecialization_ReturnsMatchingDoctors()
        {
            var specializationId = Guid.NewGuid();

            _context.Employees.AddRange(
                CreateDoctor(specializationId),
                CreateDoctor(Guid.NewGuid()), // different specialization
                CreateNonDoctor()
            );
            await _context.SaveChangesAsync();

            var result = await _repo.GetDoctorsAsync(specializationId);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(specializationId, result.First().DoctorDetails!.SpecializationId);
        }

        [TestMethod]
        public async Task GetDoctorsAsync_NoMatchingSpecialization_ReturnsEmptyList()
        {
            _context.Employees.Add(CreateDoctor(Guid.NewGuid()));
            await _context.SaveChangesAsync();

            var result = await _repo.GetDoctorsAsync(Guid.NewGuid());

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetDoctorWithDetailsAsync_DoctorExists_ReturnsDoctorWithDetails()
        {
            var specialization = new DoctorSpecialization
            {
                Id = Guid.NewGuid(),
                Type = "Cardiology"
            };

            var doctor = CreateDoctor();
            doctor.DoctorDetails!.DoctorSpecialization = specialization;

            _context.DoctorSpecializations.Add(specialization);
            _context.Employees.Add(doctor);
            await _context.SaveChangesAsync();

            var result = await _repo.GetDoctorWithDetailsAsync(doctor.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(EmployeeRole.Doctor, result.Role);
            Assert.IsNotNull(result.DoctorDetails);
            Assert.IsNotNull(result.DoctorDetails.DoctorSpecialization);
            Assert.AreEqual("Cardiology", result.DoctorDetails.DoctorSpecialization.Type);
        }

        [TestMethod]
        public async Task GetDoctorWithDetailsAsync_EmployeeIsNotDoctor_ReturnsNull()
        {
            var employee = CreateNonDoctor();
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var result = await _repo.GetDoctorWithDetailsAsync(employee.Id);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetDoctorWithDetailsAsync_DoctorDoesNotExist_ReturnsNull()
        {
            var result = await _repo.GetDoctorWithDetailsAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }
    }
}


