using ClinicCare.Api.Controllers;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ClinicCare.UnitTest.Controllers
{
    [TestClass]
    public class EmployeesControllerTests
    {
        private readonly Mock<IEmployeeService> _mockService;
        private readonly EmployeesController _controller;

        public EmployeesControllerTests()
        {
            _mockService = new Mock<IEmployeeService>();
            _controller = new EmployeesController(_mockService.Object);
        }

        [TestMethod]
        public async Task LoginAsync_ValidCredentials_ReturnsOk()
        {
            var dto = new EmployeeLoginDto
            {
                Email = "admin@test.com",
                Password = "Password@123"
            };

            var response = new EmployeeLoginResponseDto
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Role = EmployeeRole.Admin
            };

            _mockService.Setup(s => s.LoginAsync(dto))
                        .ReturnsAsync(response);

            var result = await _controller.LoginAsync(dto);

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.IsInstanceOfType(ok.Value, typeof(EmployeeLoginResponseDto));
        }

        [TestMethod]
        public async Task RegisterAsync_ValidDto_ReturnsCreatedAtRoute()
        {
            var dto = new EmployeeRegisterDto();
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.RegisterAsync(dto)).ReturnsAsync(id);

            var result = await _controller.RegisterAsync(dto);

            var created = result as CreatedAtRouteResult;
            Assert.IsNotNull(created);
            Assert.AreEqual("GetEmployeeById", created.RouteName);
            Assert.AreEqual(id, created.RouteValues!["id"]);
        }

        [TestMethod]
        public async Task GetAllAsync_ReturnsOk()
        {
            _mockService
                .Setup(s => s.GetAllAsync(null))
                .ReturnsAsync(new List<EmployeeResponseDto>());

            var result = await _controller.GetAllAsync(null);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task GetByIdAsync_ExistingId_ReturnsOk()
        {
            var id = Guid.NewGuid();

            _mockService
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(new EmployeeResponseDto { Id = id });

            var result = await _controller.GetByIdAsync(id);

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
        }

        [TestMethod]
        public async Task UpdatePutAsync_ValidRequest_ReturnsNoContent()
        {
            var id = Guid.NewGuid();
            var dto = new EmployeeUpdateDto();
            _mockService.Setup(s => s.UpdateAsync(id, dto)).Returns(Task.CompletedTask);

            var result = await _controller.UpdatePutAsync(id, dto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task UpdatePatchAsync_ValidRequest_ReturnsNoContent()
        {
            var id = Guid.NewGuid();
            var dto = new EmployeeUpdateDto();
            _mockService.Setup(s => s.UpdateAsync(id, dto)).Returns(Task.CompletedTask);

            var result = await _controller.UpdatePatchAsync(id, dto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task DeleteAsync_ExistingId_ReturnsNoContent()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteAsync(id))
                        .Returns(Task.CompletedTask);

            var result = await _controller.DeleteAsync(id);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}