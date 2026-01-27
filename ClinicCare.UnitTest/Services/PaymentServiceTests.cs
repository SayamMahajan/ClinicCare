using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Payment;
using ClinicCare.Shared.Enums;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace ClinicCare.UnitTest.Services
{
    [TestClass]
    public class PaymentServiceTests
    {
        private Mock<IGenericRepository<Payment>> _repoMock = null!;
        private Mock<IPaymentRepository> _paymentRepoMock = null!;
        private Mock<ICurrentUser> _currentUserMock = null!;
        private PaymentService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repoMock = new Mock<IGenericRepository<Payment>>();
            _paymentRepoMock = new Mock<IPaymentRepository>();
            _currentUserMock = new Mock<ICurrentUser>();

            _service = new PaymentService(
                _repoMock.Object,
                _paymentRepoMock.Object,
                _currentUserMock.Object
            );
        }

        [TestMethod]
        public async Task GetAllAsync_Admin_ReturnsAllPayments()
        {
            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Admin);

            var payment = CreateValidPayment(Guid.NewGuid(), Guid.NewGuid());

            _repoMock.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(new List<Payment> { payment });

            var result = await _service.GetAllAsync();

            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public async Task GetAllAsync_Doctor_ReturnsDoctorPayments()
        {
            var doctorId = Guid.NewGuid();

            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Doctor);
            _currentUserMock.SetupGet(c => c.UserId).Returns(doctorId);

            var payment = CreateValidPayment(Guid.NewGuid(), doctorId);

            _paymentRepoMock.Setup(r => r.GetPaymentsForDoctorAsync(doctorId))
                            .ReturnsAsync(new List<Payment> { payment });

            var result = await _service.GetAllAsync();

            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public async Task GetAllAsync_Patient_ReturnsPatientPayments()
        {
            var patientId = Guid.NewGuid();

            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Patient);
            _currentUserMock.SetupGet(c => c.UserId).Returns(patientId);

            var payment = CreateValidPayment(patientId, Guid.NewGuid());

            _paymentRepoMock.Setup(r => r.GetPaymentsForPatientAsync(patientId))
                            .ReturnsAsync(new List<Payment> { payment });

            var result = await _service.GetAllAsync();

            Assert.AreEqual(1, result.Count());
        }


        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public async Task GetAllAsync_InvalidRole_ThrowsForbidden()
        {
            _currentUserMock.SetupGet(c => c.Role).Returns((UserRole)99);

            await _service.GetAllAsync();
        }

        [TestMethod]
        public async Task GetByIdAsync_Valid_ReturnsPayment()
        {
            var senderId = Guid.NewGuid();
            var recipientId = Guid.NewGuid();

            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Patient);
            _currentUserMock.SetupGet(c => c.UserId).Returns(senderId);

            var payment = CreateValidPayment(senderId, recipientId);

            _repoMock.Setup(r => r.GetByIdAsync(payment.Id))
                     .ReturnsAsync(payment);

            var result = await _service.GetByIdAsync(payment.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(payment.Id, result.Id);
        }


        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public async Task GetByIdAsync_InvalidId_ThrowsNotFound()
        {
            var id = Guid.NewGuid();

            _repoMock.Setup(r => r.GetByIdAsync(id))
                     .ReturnsAsync((Payment?)null);

            try
            {
                await _service.GetByIdAsync(id);
            }
            catch (NotFoundException ex)
            {
                Assert.AreEqual($"Payment with id {id} not found.", ex.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public async Task GetByIdAsync_PatientAccessingOthersPayment_ThrowsForbidden()
        {
            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Patient);
            _currentUserMock.SetupGet(c => c.UserId).Returns(Guid.NewGuid());

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                SenderId = Guid.NewGuid(),
                RecipientId = Guid.NewGuid()
            };

            _repoMock.Setup(r => r.GetByIdAsync(payment.Id))
                     .ReturnsAsync(payment);

            await _service.GetByIdAsync(payment.Id);
        }

        [TestMethod]
        public async Task CreateAsync_ValidPayment_CreatesPayment()
        {
            var senderId = Guid.NewGuid();
            var recipientId = Guid.NewGuid();

            _currentUserMock.SetupGet(c => c.UserId).Returns(senderId);

            Payment? inserted = null;

            _repoMock.Setup(r => r.InsertAsync(It.IsAny<Payment>()))
                     .Callback<Payment>(p => inserted = p)
                     .Returns(Task.CompletedTask);

            _repoMock.Setup(r => r.SaveChangesAsync())
                     .Returns(Task.CompletedTask);

            var dto = new PaymentCreateDto
            {
                SenderId = senderId,
                RecipientId = recipientId,
                Amount = 500
            };

            var result = await _service.CreateAsync(dto);

            Assert.IsNotNull(inserted);
            Assert.AreEqual(result, inserted!.Id);
            Assert.AreEqual(senderId, inserted.SenderId);
            Assert.AreEqual(recipientId, inserted.RecipientId);

            _repoMock.Verify(r => r.InsertAsync(It.IsAny<Payment>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public async Task CreateAsync_SenderMismatch_ThrowsForbidden()
        {
            _currentUserMock.SetupGet(c => c.UserId).Returns(Guid.NewGuid());

            var dto = new PaymentCreateDto
            {
                SenderId = Guid.NewGuid(),
                RecipientId = Guid.NewGuid(),
                Amount = 100
            };

            await _service.CreateAsync(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public async Task CreateAsync_SameSenderAndRecipient_ThrowsValidation()
        {
            var id = Guid.NewGuid();
            _currentUserMock.SetupGet(c => c.UserId).Returns(id);

            var dto = new PaymentCreateDto
            {
                SenderId = id,
                RecipientId = id,
                Amount = 100
            };

            await _service.CreateAsync(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public async Task CreateAsync_ZeroAmount_ThrowsValidation()
        {
            var id = Guid.NewGuid();
            _currentUserMock.SetupGet(c => c.UserId).Returns(id);

            var dto = new PaymentCreateDto
            {
                SenderId = id,
                RecipientId = Guid.NewGuid(),
                Amount = 0
            };

            await _service.CreateAsync(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public async Task CreateAsync_AmountExceedsLimit_ThrowsValidation()
        {
            var id = Guid.NewGuid();
            _currentUserMock.SetupGet(c => c.UserId).Returns(id);

            var dto = new PaymentCreateDto
            {
                SenderId = id,
                RecipientId = Guid.NewGuid(),
                Amount = 20_000_000
            };

            await _service.CreateAsync(dto);
        }

        private static Payment CreateValidPayment(
            Guid senderId,
            Guid recipientId)
        {
            return new Payment
            {
                Id = Guid.NewGuid(),
                Amount = 500,
                SenderId = senderId,
                RecipientId = recipientId,
                Sender = new Patient
                {
                    Id = senderId,
                    FirstName = "John",
                    LastName = "Doe"
                },
                Recipient = new Employee
                {
                    Id = recipientId,
                    FirstName = "Dr",
                    LastName = "Smith"
                },
                CreatedAt = DateTime.UtcNow
            };
        }

    }
}
