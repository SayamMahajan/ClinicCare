using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Payment;
using ClinicCare.Shared.Enums;
using Moq;

namespace ClinicCare.UnitTest.Services
{
    [TestClass]
    public class PaymentServiceTests
    {
        private readonly Mock<IGenericRepository<Payment>> _repoMock;
        private readonly Mock<IPaymentRepository> _paymentRepoMock;
        private readonly Mock<ICurrentUser> _currentUserMock;
        private readonly PaymentService _service;

        public PaymentServiceTests()
        {
            _repoMock = new Mock<IGenericRepository<Payment>>();
            _paymentRepoMock = new Mock<IPaymentRepository>();
            _currentUserMock = new Mock<ICurrentUser>();
            _service = new PaymentService(
                _repoMock.Object,
                _paymentRepoMock.Object,
                _currentUserMock.Object);
        }

        [TestMethod]
        public async Task GetAllAsync_Admin_ReturnsAllPayments()
        {
            _currentUserMock.Setup(c => c.Role).Returns(UserRole.Admin);

            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe"
            };

            var doctor = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith"
            };

            var payments = new List<Payment>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Amount = 500,
                    Sender = patient,
                    SenderId = patient.Id,
                    Recipient = doctor,
                    RecipientId = doctor.Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Amount = 800,
                    Sender = patient,
                    SenderId = patient.Id,
                    Recipient = doctor,
                    RecipientId = doctor.Id
                }
            };

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(payments);

            var result = await _service.GetAllAsync();

            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task GetAllAsync_Doctor_ReturnsDoctorPayments()
        {
            var doctorId = Guid.NewGuid();
            _currentUserMock.Setup(c => c.Role).Returns(UserRole.Doctor);
            _currentUserMock.Setup(c => c.UserId).Returns(doctorId);

            _paymentRepoMock
                .Setup(r => r.GetPaymentsForDoctorAsync(doctorId))
                .ReturnsAsync(new List<Payment>());

            var result = await _service.GetAllAsync();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public async Task GetByIdAsync_InvalidId_ThrowsNotFound()
        {
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Payment?)null);

            await _service.GetByIdAsync(id);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public async Task GetByIdAsync_PatientAccessingOthersPayment_ThrowsForbidden()
        {
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                SenderId = Guid.NewGuid()
            };

            _currentUserMock.Setup(c => c.Role).Returns(UserRole.Patient);
            _currentUserMock.Setup(c => c.UserId).Returns(Guid.NewGuid());

            _repoMock.Setup(r => r.GetByIdAsync(payment.Id)).ReturnsAsync(payment);

            await _service.GetByIdAsync(payment.Id);
        }

        [TestMethod]
        public async Task CreateAsync_ValidRequest_ShouldInsertSaveAndReturnId()
        {
            var dto = new PaymentCreateDto
            {
                Amount = 1000,
                SenderId = Guid.NewGuid(),
                RecipientId = Guid.NewGuid()
            };

            _repoMock.Setup(r => r.InsertAsync(It.IsAny<Payment>()))
                     .Returns(Task.CompletedTask);

            _repoMock.Setup(r => r.SaveChangesAsync())
                     .Returns(Task.CompletedTask);

            var id = await _service.CreateAsync(dto);

            Assert.AreNotEqual(Guid.Empty, id);
            _repoMock.Verify(r => r.InsertAsync(It.IsAny<Payment>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}