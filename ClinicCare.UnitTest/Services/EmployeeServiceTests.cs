using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Helpers;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services;
using ClinicCare.Business.Utils;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.Enums;
using Moq;

namespace ClinicCare.UnitTest.Services
{
    [TestClass]
    public class EmployeeServiceTests
    {
        private Mock<IGenericRepository<Employee>> _repoMock = null!;
        private Mock<IEmployeeRepository> _employeeRepoMock = null!;
        private Mock<IGenericRepository<DoctorSpecialization>> _specializationRepoMock = null!;
        private Mock<IJwtTokenGenerator> _jwtMock = null!;
        private Mock<ICurrentUser> _currentUserMock = null!;
        private EmployeeService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repoMock = new Mock<IGenericRepository<Employee>>();
            _employeeRepoMock = new Mock<IEmployeeRepository>();
            _specializationRepoMock = new Mock<IGenericRepository<DoctorSpecialization>>();
            _jwtMock = new Mock<IJwtTokenGenerator>();
            _currentUserMock = new Mock<ICurrentUser>();

            _service = new EmployeeService(
                _repoMock.Object,
                _employeeRepoMock.Object,
                _specializationRepoMock.Object,
                _jwtMock.Object,
                _currentUserMock.Object
            );
        }

        [TestMethod]
        public async Task LoginAsync_Valid_ReturnsToken()
        {
            var dto = new EmployeeLoginDto
            {
                Email = "test@mail.com",
                Password = "Password@123"
            };

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = dto.Email,
                Password = PasswordHelper.Hash(dto.Password),
                Role = EmployeeRole.Doctor
            };

            _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>()))
                     .ReturnsAsync(new[] { employee });

            _jwtMock.Setup(j => j.GenerateEmployeeToken(employee))
                    .Returns("token123");

            var result = await _service.LoginAsync(dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(employee.Id, result.Id);
            Assert.AreEqual("token123", result.Token);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedException))]
        public async Task LoginAsync_InvalidPassword_ThrowsUnauthorized()
        {
            var dto = new EmployeeLoginDto
            {
                Email = "test@mail.com",
                Password = "wrong"
            };

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Password = PasswordHelper.Hash("Password@123")
            };

            _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>()))
                     .ReturnsAsync(new[] { employee });

            await _service.LoginAsync(dto);
        }

        [TestMethod]
        public async Task RegisterAsync_ValidDoctor_CreatesEmployee()
        {
            var dto = new EmployeeRegisterDto
            {
                Email = "doctor@mail.com",
                FirstName = "John",
                LastName = "Doe",
                Password = "Password@123",
                Role = EmployeeRole.Doctor,
                DateOfJoining = DateTime.UtcNow.AddDays(-1),
                DoctorDetails = new DoctorRegisterDetailsDto
                {
                    SpecializationId = Guid.NewGuid(),
                    Fee = 100,
                    DOB = DateTime.UtcNow.AddYears(-30),
                    FirstPracticeDate = DateTime.UtcNow.AddYears(-5),
                    Phone = "1234567890"
                }
            };

            _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>()))
                     .ReturnsAsync(Array.Empty<Employee>());

            _specializationRepoMock.Setup(s => s.GetByIdAsync(dto.DoctorDetails.SpecializationId))
                                   .ReturnsAsync(new DoctorSpecialization { Id = dto.DoctorDetails.SpecializationId });

            Employee? inserted = null;
            _repoMock.Setup(r => r.InsertAsync(It.IsAny<Employee>()))
                     .Callback<Employee>(e => inserted = e)
                     .Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.RegisterAsync(dto);

            Assert.IsNotNull(inserted);
            Assert.AreEqual(result, inserted!.Id);
            Assert.AreEqual(dto.Role, inserted.Role);
            Assert.AreEqual(dto.DoctorDetails.SpecializationId, inserted.DoctorDetails!.SpecializationId);
        }

        [TestMethod]
        [ExpectedException(typeof(ConflictException))]
        public async Task RegisterAsync_EmailExists_ThrowsConflict()
        {
            var dto = new EmployeeRegisterDto
            {
                Email = "exists@mail.com",
                FirstName = "John",
                LastName = "Doe",
                Password = "Password@123",
                Role = EmployeeRole.Doctor,
                DateOfJoining = DateTime.UtcNow.AddDays(-1)
            };

            _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>()))
                     .ReturnsAsync(new[] { new Employee() });

            await _service.RegisterAsync(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task RegisterAsync_DoctorWithoutDetails_ThrowsBadRequest()
        {
            var dto = new EmployeeRegisterDto
            {
                Email = "doc@mail.com",
                FirstName = "John",
                LastName = "Doe",
                Password = "Password@123",
                Role = EmployeeRole.Doctor,
                DateOfJoining = DateTime.UtcNow.AddDays(-1),
                DoctorDetails = null
            };

            _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>()))
                     .ReturnsAsync(Array.Empty<Employee>());

            await _service.RegisterAsync(dto);
        }

        [TestMethod]
        public async Task GetAllAsync_NoRole_ReturnsAllEmployees()
        {
            var employees = new List<Employee> { new Employee { Id = Guid.NewGuid() } };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(employees);

            var result = await _service.GetAllAsync(null);

            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public async Task GetAllAsync_DoctorRole_CallsGetDoctors()
        {
            var doctorId = Guid.NewGuid();
            var employees = new List<Employee> { new Employee { Id = doctorId } };
            _employeeRepoMock.Setup(r => r.GetDoctorsAsync(null)).ReturnsAsync(employees);

            var result = await _service.GetAllAsync(EmployeeRole.Doctor);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(doctorId, result.First().Id);
        }

        [TestMethod]
        public async Task GetByIdAsync_Valid_ReturnsEmployee()
        {
            var id = Guid.NewGuid();
            var employee = new Employee { Id = id, Role = EmployeeRole.Doctor };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(employee);

            var result = await _service.GetByIdAsync(id);

            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public async Task GetByIdAsync_AdminRoleAccessDenied_ThrowsForbidden()
        {
            var id = Guid.NewGuid();
            _currentUserMock.SetupGet(c => c.Role).Returns(UserRole.Patient);
            var employee = new Employee { Id = id, Role = EmployeeRole.Admin };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(employee);

            await _service.GetByIdAsync(id);
        }

        [TestMethod]
        public async Task UpdateAsync_Valid_UpdatesEmployee()
        {
            var id = Guid.NewGuid();
            var employee = new Employee { Id = id, Role = EmployeeRole.Doctor, FirstName = "Old" };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(employee);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new EmployeeUpdateDto { FirstName = "New" };
            await _service.UpdateAsync(id, dto);

            Assert.AreEqual(NormalizationHelper.NormalizeKey("New"), employee.FirstName);

        }

        [TestMethod]
        public async Task DeleteAsync_Valid_DeletesEmployee()
        {
            var id = Guid.NewGuid();
            var employee = new Employee { Id = id };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(employee);
            _repoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.DeleteAsync(id);

            _repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
