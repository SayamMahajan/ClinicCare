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
        private Mock<IGenericRepository<DoctorSpecialization>> _repoMock = null!;
        private SpecializationService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repoMock = new Mock<IGenericRepository<DoctorSpecialization>>();
            _service = new SpecializationService(_repoMock.Object, null!);
        }

        [TestMethod]
        public async Task GetAllAsync_ValidRequest_ReturnsMappedDtos()
        {
            var data = new List<DoctorSpecialization>
            {
                new() { Id = Guid.NewGuid(), Type = "cardiology" },
                new() { Id = Guid.NewGuid(), Type = "neurology" }
            };

            _repoMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(data);

            var result = await _service.GetAllAsync();

            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(s => s.Type == "cardiology"));
            Assert.IsTrue(result.Any(s => s.Type == "neurology"));
        }

        [TestMethod]
        public async Task GetByIdAsync_ExistingId_ReturnsDto()
        {
            var id = Guid.NewGuid();
            var entity = new DoctorSpecialization
            {
                Id = id,
                Type = "orthopedics"
            };

            _repoMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(id);

            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.Id);
            Assert.AreEqual("orthopedics", result.Type);
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
        public async Task CreateAsync_ValidData_CreatesSpecialization()
        {
            // Arrange
            var dto = new SpecializationCreateDto
            {
                Type = "  Cardiology "
            };

            _repoMock
                .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<DoctorSpecialization, bool>>>()))
                .ReturnsAsync(Enumerable.Empty<DoctorSpecialization>());

            DoctorSpecialization? insertedEntity = null;

            _repoMock
                .Setup(r => r.InsertAsync(It.IsAny<DoctorSpecialization>()))
                .Callback<DoctorSpecialization>(s => insertedEntity = s)
                .Returns(Task.CompletedTask);

            _repoMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(dto);

            Assert.AreNotEqual(Guid.Empty, result);
            Assert.IsNotNull(insertedEntity);

            Assert.AreEqual("cardiology", insertedEntity!.Type);

            Assert.AreEqual(insertedEntity.Id, result);

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