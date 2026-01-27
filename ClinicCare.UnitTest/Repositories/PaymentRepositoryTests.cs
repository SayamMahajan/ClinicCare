using ClinicCare.DataAccess.Data;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories;
using ClinicCare.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClinicCare.UnitTest.Repositories
{
    [TestClass]
    public class PaymentRepositoryTests
    {
        private AppDbContext _context = null!;
        private PaymentRepository _repo = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repo = new PaymentRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private static Patient CreatePatient()
        {
            return new Patient
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Patient",
                DOB = DateTime.UtcNow.AddYears(-30),
                Gender = Gender.Male,
                Email = $"patient{Guid.NewGuid()}@test.com",
                Password = "Password@123",
                Phone = "9999999999"
            };
        }

        private static Employee CreateDoctor()
        {
            return new Employee
            {
                Id = Guid.NewGuid(),
                Role = EmployeeRole.Doctor,
                FirstName = "Dr",
                LastName = "Smith",
                Email = $"doctor{Guid.NewGuid()}@test.com",
                Password = "Password@123",
                DateOfJoining = DateTime.UtcNow
            };
        }

        private static Payment CreatePayment(
            Guid senderId,
            Guid recipientId,
            Patient sender,
            Employee recipient)
        {
            return new Payment
            {
                Id = Guid.NewGuid(),
                Amount = 500,
                SenderId = senderId,
                RecipientId = recipientId,
                Sender = sender,
                Recipient = recipient,
                CreatedAt = DateTime.UtcNow
            };
        }

        [TestMethod]
        public async Task GetPaymentsForDoctorAsync_ReturnsPayments_ForGivenDoctor()
        {
            var patient = CreatePatient();
            var doctor1 = CreateDoctor();
            var doctor2 = CreateDoctor();

            var p1 = CreatePayment(patient.Id, doctor1.Id, patient, doctor1);
            var p2 = CreatePayment(patient.Id, doctor2.Id, patient, doctor2);

            _context.Patients.Add(patient);
            _context.Employees.AddRange(doctor1, doctor2);
            _context.Payments.AddRange(p1, p2);
            await _context.SaveChangesAsync();

            var result = (await _repo.GetPaymentsForDoctorAsync(doctor1.Id)).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(doctor1.Id, result.First().RecipientId);
            Assert.IsNotNull(result.First().Sender);
            Assert.IsNotNull(result.First().Recipient);
        }

        [TestMethod]
        public async Task GetPaymentsForDoctorAsync_NoMatch_ReturnsEmptyList()
        {
            var result = await _repo.GetPaymentsForDoctorAsync(Guid.NewGuid());

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetPaymentsForPatientAsync_ReturnsPayments_ForGivenPatient()
        {
            var patient1 = CreatePatient();
            var patient2 = CreatePatient();
            var doctor = CreateDoctor();

            var p1 = CreatePayment(patient1.Id, doctor.Id, patient1, doctor);
            var p2 = CreatePayment(patient2.Id, doctor.Id, patient2, doctor);

            _context.Patients.AddRange(patient1, patient2);
            _context.Employees.Add(doctor);
            _context.Payments.AddRange(p1, p2);
            await _context.SaveChangesAsync();

            var result = (await _repo.GetPaymentsForPatientAsync(patient1.Id)).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(patient1.Id, result.First().SenderId);
            Assert.IsNotNull(result.First().Sender);
            Assert.IsNotNull(result.First().Recipient);
        }

        [TestMethod]
        public async Task GetPaymentsForPatientAsync_NoMatch_ReturnsEmptyList()
        {
            var result = await _repo.GetPaymentsForPatientAsync(Guid.NewGuid());

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }
    }
}
