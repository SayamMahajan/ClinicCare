using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Prescription;
using ClinicCare.Shared.Enums;
using Moq;
using System.Text.Json;

namespace ClinicCare.UnitTest.Services
{
    [TestClass]
    public class PrescriptionServiceTests
    {
        private Mock<IGenericRepository<Prescription>> _repoMock = null!;
        private Mock<IGenericRepository<Patient>> _patientRepoMock = null!;
        private Mock<ICurrentUser> _currentUserMock = null!;
        private PrescriptionService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repoMock = new Mock<IGenericRepository<Prescription>>();
            _patientRepoMock = new Mock<IGenericRepository<Patient>>();
            _currentUserMock = new Mock<ICurrentUser>();

            _service = new PrescriptionService(
                _repoMock.Object,
                _patientRepoMock.Object,
                _currentUserMock.Object
            );
        }

        [TestMethod]
        public async Task GetAllAsync_Doctor_ReturnsDoctorPrescriptions()
        {
            var doctorId = Guid.NewGuid();

            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Doctor);
            _currentUserMock.SetupGet(c => c.UserId).Returns(doctorId);

            var data = new List<Prescription>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    DoctorId = doctorId,
                    PatientId = Guid.NewGuid(),
                    Description = JsonSerializer.Serialize(new List<MedicationDto>())
                }
            };

            _repoMock
                .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Prescription, bool>>>()))
                .ReturnsAsync(data);

            var result = await _service.GetAllAsync();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(doctorId, result.First().DoctorId);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public async Task GetAllAsync_InvalidRole_ThrowsForbidden()
        {
            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Admin);

            await _service.GetAllAsync();
        }

        [TestMethod]
        public async Task GetByIdAsync_ValidId_ReturnsDto()
        {
            var id = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var doctorId = Guid.NewGuid();

            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Doctor);
            _currentUserMock.SetupGet(c => c.UserId).Returns(doctorId);

            var entity = new Prescription
            {
                Id = id,
                PatientId = patientId,
                DoctorId = doctorId,
                Description = JsonSerializer.Serialize(new List<MedicationDto>())
            };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(id);

            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.Id);
            Assert.AreEqual(patientId, result.PatientId);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public async Task GetByIdAsync_InvalidId_ThrowsNotFound()
        {
            var id = Guid.NewGuid();

            _repoMock.Setup(r => r.GetByIdAsync(id))
                     .ReturnsAsync((Prescription?)null);

            try
            {
                await _service.GetByIdAsync(id);
            }
            catch (NotFoundException ex)
            {
                Assert.AreEqual($"Prescription with id {id} not found.", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public async Task CreateAsync_ValidDoctor_CreatesPrescription()
        {
            var doctorId = Guid.NewGuid();
            var patientId = Guid.NewGuid();

            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Doctor);
            _currentUserMock.SetupGet(c => c.UserId).Returns(doctorId);

            _patientRepoMock
                .Setup(r => r.GetByIdAsync(patientId))
                .ReturnsAsync(new Patient { Id = patientId });

            Prescription? insertedEntity = null;

            _repoMock
                .Setup(r => r.InsertAsync(It.IsAny<Prescription>()))
                .Callback<Prescription>(p => insertedEntity = p)
                .Returns(Task.CompletedTask);

            _repoMock.Setup(r => r.SaveChangesAsync())
                     .Returns(Task.CompletedTask);

            var dto = new PrescriptionCreateDto
            {
                DoctorId = doctorId,
                PatientId = patientId,
                Description = new List<MedicationDto>
                {
                    new() { Medicine = "Paracetamol", Dosage = 1, Days = 2, Frequency = "3" }
                }
            };

            var result = await _service.CreateAsync(dto);

            Assert.IsNotNull(insertedEntity);
            Assert.AreEqual(result, insertedEntity!.Id);
            Assert.AreEqual(doctorId, insertedEntity.DoctorId);
            Assert.AreEqual(patientId, insertedEntity.PatientId);

            _repoMock.Verify(r => r.InsertAsync(It.IsAny<Prescription>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public async Task CreateAsync_NonDoctor_ThrowsForbidden()
        {
            // Arrange
            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Patient);
            _currentUserMock.SetupGet(c => c.UserId).Returns(Guid.NewGuid());

            var dto = new PrescriptionCreateDto
            {
                PatientId = Guid.NewGuid(),
                DoctorId = Guid.NewGuid(),
                Description = new List<MedicationDto>
                {
                    new() { Medicine = "Paracetamol", Dosage = 1, Days = 2, Frequency = "3" }
                }
            };

            await _service.CreateAsync(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task CreateAsync_InvalidDto_ThrowsBadRequest()
        {
            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Doctor);
            _currentUserMock.SetupGet(c => c.UserId).Returns(Guid.NewGuid());

            try
            {
                await _service.CreateAsync(new PrescriptionCreateDto());
            }
            catch (BadRequestException ex)
            {
                Assert.AreEqual("PatientId is invalid.", ex.Message);
                throw;
            }
        }


        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public async Task CreateAsync_DoctorCreatingForAnotherDoctor_ThrowsForbidden()
        {
            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Doctor);
            _currentUserMock.SetupGet(c => c.UserId).Returns(Guid.NewGuid());

            var dto = new PrescriptionCreateDto
            {
                DoctorId = Guid.NewGuid(),
                PatientId = Guid.NewGuid()
            };

            await _service.CreateAsync(dto);
        }
    }
}
