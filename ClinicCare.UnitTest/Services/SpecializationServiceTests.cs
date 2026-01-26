using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Services;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Specialization;
using Moq;

namespace ClinicCare.UnitTest.Services
{
    [TestClass]
    public class SpecializationServiceTests
    {
        private readonly Mock<IGenericRepository<DoctorSpecialization>> _repoMock;
        private readonly SpecializationService _service;

        public SpecializationServiceTests()
        {
            _repoMock = new Mock<IGenericRepository<DoctorSpecialization>>();
            _service = new SpecializationService(_repoMock.Object, null!);
        }

        [TestMethod]
        public async Task GetAllAsync_ValidRequest_ReturnsSpecializationResponseDto()
        {
            var data = new List<DoctorSpecialization>
            {
                new() { Id = Guid.NewGuid(), Type = "Cardiologist" },
                new() { Id = Guid.NewGuid(), Type = "ENT" }
            };

            _repoMock.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(data);

            var result = await _service.GetAllAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());

            Assert.AreEqual(data[0].Id, result.ElementAt(0).Id);
            Assert.AreEqual(data[0].Type, result.ElementAt(0).Type);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public async Task GetByIdAsync_InvalidId_ThrowNotFound()
        {
            var id = Guid.NewGuid();

            _repoMock.Setup(r => r.GetByIdAsync(id))
                     .ReturnsAsync((DoctorSpecialization?)null);

            try
            {
                await _service.GetByIdAsync(id);
            }
            catch (NotFoundException ex)
            {
                Assert.AreEqual($"Specialization with id {id} not found.", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public async Task GetByIdAsync_ValidId_ReturnsSpecializationResponseDto()
        {
            // Arrange
            var specialization = new DoctorSpecialization
            {
                Id = Guid.NewGuid(),
                Type = "Neurologist"
            };

            _repoMock.Setup(r => r.GetByIdAsync(specialization.Id))
                     .ReturnsAsync(specialization);

            // Act
            var result = await _service.GetByIdAsync(specialization.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(specialization.Id, result.Id);
            Assert.AreEqual(specialization.Type, result.Type);
        }

        [TestMethod]
        public async Task CreateAsync_ValidRequest_ShouldInsertSaveAndReturnId()
        {
            var dto = new SpecializationCreateDto
            {
                Type = "Psychiatrist"
            };

            DoctorSpecialization? capturedEntity = null;

            _repoMock.Setup(r => r.InsertAsync(It.IsAny<DoctorSpecialization>()))
                     .Callback<DoctorSpecialization>(e => capturedEntity = e)
                     .Returns(Task.CompletedTask);

            _repoMock.Setup(r => r.SaveChangesAsync())
                     .Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(dto);

            Assert.IsNotNull(capturedEntity);
            Assert.AreEqual(dto.Type, capturedEntity.Type);
            Assert.AreNotEqual(Guid.Empty, result);

            _repoMock.Verify(r => r.InsertAsync(It.IsAny<DoctorSpecialization>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public async Task DeleteAsync_InvalidId_ThrowNotFound()
        {
            var id = Guid.NewGuid();

            _repoMock.Setup(r => r.GetByIdAsync(id))
                     .ReturnsAsync((DoctorSpecialization?)null);

            try
            {
                await _service.DeleteAsync(id);
            }
            catch (NotFoundException ex)
            {
                Assert.AreEqual($"Specialization with id {id} not found.", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public async Task DeleteAsync_ValidId_ShouldDeleteAndSave()
        {
            // Arrange
            var specialization = new DoctorSpecialization
            {
                Id = Guid.NewGuid(),
                Type = "Dermatologist"
            };

            _repoMock.Setup(r => r.GetByIdAsync(specialization.Id))
                     .ReturnsAsync(specialization);

            _repoMock.Setup(r => r.DeleteAsync(specialization.Id))
                     .Returns(Task.CompletedTask);

            _repoMock.Setup(r => r.SaveChangesAsync())
                     .Returns(Task.CompletedTask);

            await _service.DeleteAsync(specialization.Id);

            _repoMock.Verify(r => r.DeleteAsync(specialization.Id), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}