using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Helpers;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services;
using ClinicCare.Business.Utils;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Patient;
using ClinicCare.Shared.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ClinicCare.UnitTest.Services
{
    [TestClass]
    public class PatientServiceTests
    {
        private Mock<IGenericRepository<Patient>> _repoMock = null!;
        private Mock<IJwtTokenGenerator> _jwtMock = null!;
        private Mock<ICurrentUser> _currentUserMock = null!;
        private PatientService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repoMock = new Mock<IGenericRepository<Patient>>();
            _jwtMock = new Mock<IJwtTokenGenerator>();
            _currentUserMock = new Mock<ICurrentUser>();

            _service = new PatientService(
                _repoMock.Object,
                _jwtMock.Object,
                _currentUserMock.Object
            );
        }

        [TestMethod]
        public async Task LoginPatientAsync_ValidCredentials_ReturnsToken()
        {
            var password = "Password@123";
            var hashed = PasswordHelper.Hash(password);

            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                Email = "test@mail.com",
                Password = hashed,
                FirstName = "John",
                LastName = "Doe"
            };

            _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Patient, bool>>>()))
                     .ReturnsAsync(new[] { patient });

            _jwtMock.Setup(j => j.GeneratePatientToken(patient))
                    .Returns("jwt-token");

            var dto = new PatientLoginDto
            {
                Email = " TEST@mail.com ",
                Password = password
            };

            var result = await _service.LoginPatientAsync(dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(patient.Id, result.Id);
            Assert.AreEqual("jwt-token", result.Token);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedException))]
        public async Task LoginPatientAsync_InvalidCredentials_ThrowsUnauthorized()
        {
            _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Patient, bool>>>()))
                     .ReturnsAsync(Array.Empty<Patient>());

            try
            {
                await _service.LoginPatientAsync(new PatientLoginDto
                {
                    Email = "wrong@mail.com",
                    Password = "123"
                });
            }
            catch (UnauthorizedException ex)
            {
                Assert.AreEqual("Invalid email or password.", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public async Task RegisterPatientAsync_ValidData_CreatesPatient()
        {
            _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Patient, bool>>>()))
                     .ReturnsAsync(Array.Empty<Patient>());

            _repoMock.Setup(r => r.InsertAsync(It.IsAny<Patient>()))
                     .Returns(Task.CompletedTask);

            _repoMock.Setup(r => r.SaveChangesAsync())
                     .Returns(Task.CompletedTask);

            var dto = new PatientRegisterDto
            {
                FirstName = " John ",
                LastName = " Doe ",
                Email = " TEST@mail.com ",
                Phone = " 9999999999 ",
                Password = "Password@123",
                DOB = DateTime.UtcNow.AddYears(-20),
                Gender = Gender.Male
            };

            var id = await _service.RegisterPatientAsync(dto);

            Assert.AreNotEqual(Guid.Empty, id);
            _repoMock.Verify(r => r.InsertAsync(It.IsAny<Patient>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ConflictException))]
        public async Task RegisterPatientAsync_EmailExists_ThrowsConflict()
        {
            _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Patient, bool>>>()))
                     .ReturnsAsync(new[] { new Patient() });

            var dto = new PatientRegisterDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "test@mail.com",
                Phone = "9999999999",
                Password = "Password@123",
                DOB = DateTime.UtcNow.AddYears(-20),
                Gender = Gender.Male
            };

            try
            {
                await _service.RegisterPatientAsync(dto);
            }
            catch (ConflictException ex)
            {
                Assert.AreEqual("Email already registered", ex.Message);
                throw;
            }
        }


        [TestMethod]
        public async Task GetAllAsync_ReturnsPatients()
        {
            _repoMock.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(new[]
                     {
                         new Patient { Id = Guid.NewGuid(), FirstName = "A" },
                         new Patient { Id = Guid.NewGuid(), FirstName = "B" }
                     });

            var result = await _service.GetAllAsync();

            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task GetByIdAsync_AdminAccess_ReturnsPatient()
        {
            var id = Guid.NewGuid();

            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Admin);

            _repoMock.Setup(r => r.GetByIdAsync(id))
                     .ReturnsAsync(new Patient { Id = id });

            var result = await _service.GetByIdAsync(id);

            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public async Task GetByIdAsync_PatientAccessingOtherPatient_ThrowsForbidden()
        {
            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Patient);
            _currentUserMock.SetupGet(c => c.UserId).Returns(Guid.NewGuid());

            var patient = new Patient { Id = Guid.NewGuid() };

            _repoMock.Setup(r => r.GetByIdAsync(patient.Id))
                     .ReturnsAsync(patient);

            await _service.GetByIdAsync(patient.Id);
        }

        [TestMethod]
        public async Task UpdateAsync_ValidPatient_UpdatesSuccessfully()
        {
            var id = Guid.NewGuid();

            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Patient);
            _currentUserMock.SetupGet(c => c.UserId).Returns(id);

            var patient = new Patient { Id = id };

            _repoMock.Setup(r => r.GetByIdAsync(id))
                     .ReturnsAsync(patient);

            _repoMock.Setup(r => r.SaveChangesAsync())
                     .Returns(Task.CompletedTask);

            await _service.UpdateAsync(id, new PatientUpdateDto
            {
                FirstName = " Updated "
            });

            Assert.AreEqual("updated", patient.FirstName);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task DeleteAsync_Admin_DeletesPatient()
        {
            var id = Guid.NewGuid();

            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Admin);

            _repoMock.Setup(r => r.GetByIdAsync(id))
                     .ReturnsAsync(new Patient { Id = id });

            _repoMock.Setup(r => r.DeleteAsync(id))
                     .Returns(Task.CompletedTask);

            _repoMock.Setup(r => r.SaveChangesAsync())
                     .Returns(Task.CompletedTask);

            await _service.DeleteAsync(id);

            _repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public async Task DeleteAsync_PatientDeletingOther_ThrowsForbidden()
        {
            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Patient);
            _currentUserMock.SetupGet(c => c.UserId).Returns(Guid.NewGuid());

            var patient = new Patient { Id = Guid.NewGuid() };

            _repoMock.Setup(r => r.GetByIdAsync(patient.Id))
                     .ReturnsAsync(patient);

            await _service.DeleteAsync(patient.Id);
        }
    }
}
