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
        private readonly Mock<IGenericRepository<Prescription>> _repoMock;
        private readonly Mock<ICurrentUser> _currentUserMock;
        private readonly PrescriptionService _service;

        public PrescriptionServiceTests()
        {
            _repoMock = new Mock<IGenericRepository<Prescription>>();
            _currentUserMock = new Mock<ICurrentUser>();
            _service = new PrescriptionService(_repoMock.Object, _currentUserMock.Object);
        }


        [TestMethod]
        public async Task GetAllAsync_Doctor_ReturnsPrescriptions()
        {
            var doctorId = Guid.NewGuid();
            _currentUserMock.Setup(c => c.Role).Returns(UserRole.Doctor);
            _currentUserMock.Setup(c => c.UserId).Returns(doctorId);

            var prescriptions = new List<Prescription>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    DoctorId = doctorId,
                    Description = JsonSerializer.Serialize(new List<MedicationDto>())
                }
            };

            _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Prescription, bool>>>()))
                     .ReturnsAsync(prescriptions);

            
            var result = await _service.GetAllAsync();

            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public async Task GetByIdAsync_PatientAccessingOthersPrescription_ThrowsForbidden()
        {
            var prescription = new Prescription
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                Description = JsonSerializer.Serialize(new List<MedicationDto>())
            };

            _currentUserMock.Setup(c => c.Role).Returns(UserRole.Patient);
            _currentUserMock.Setup(c => c.UserId).Returns(Guid.NewGuid());

            _repoMock.Setup(r => r.GetByIdAsync(prescription.Id))
                     .ReturnsAsync(prescription);
            

            await _service.GetByIdAsync(prescription.Id);
        }

        [TestMethod]
        public async Task CreateAsync_ValidRequest_ShouldInsertSaveAndReturnId()
        {
            var dto = new PrescriptionCreateDto
            {
                DoctorId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                Description = new List<MedicationDto>()
            };

            _repoMock.Setup(r => r.InsertAsync(It.IsAny<Prescription>()))
                     .Returns(Task.CompletedTask);

            _repoMock.Setup(r => r.SaveChangesAsync())
                     .Returns(Task.CompletedTask);
            
            var id = await _service.CreateAsync(dto);

            Assert.AreNotEqual(Guid.Empty, id);
            _repoMock.Verify(r => r.InsertAsync(It.IsAny<Prescription>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task DeleteAsync_ValidId_ShouldDeleteAndSave()
        {
            var prescription = new Prescription { Id = Guid.NewGuid() };

            _repoMock.Setup(r => r.GetByIdAsync(prescription.Id))
                     .ReturnsAsync(prescription);

            _repoMock.Setup(r => r.DeleteAsync(prescription.Id))
                     .Returns(Task.CompletedTask);

            _repoMock.Setup(r => r.SaveChangesAsync())
                     .Returns(Task.CompletedTask);

            await _service.DeleteAsync(prescription.Id);

            _repoMock.Verify(r => r.DeleteAsync(prescription.Id), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}