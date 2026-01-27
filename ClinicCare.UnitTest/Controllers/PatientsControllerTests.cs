using ClinicCare.Api.Controllers;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Shared.DTOs.Patient;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ClinicCare.UnitTest.Controllers
{
    [TestClass]
    public class PatientsControllerTests
    {
        private readonly Mock<IPatientService> _mockService;
        private readonly PatientsController _controller;

        public PatientsControllerTests()
        {
            _mockService = new Mock<IPatientService>();
            _controller = new PatientsController(_mockService.Object);
        }

        [TestMethod]
        public async Task LoginPatientAsync_ValidCredentials_ReturnsOk()
        {
            var dto = new PatientLoginDto
            {
                Email = "patient@test.com",
                Password = "Password@123"
            };

            _mockService
                .Setup(s => s.LoginPatientAsync(dto))
                .ReturnsAsync(new PatientLoginResponseDto());

            var result = await _controller.LoginPatientAsync(dto);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task RegisterPatient_ValidDto_ReturnsCreated()
        {
            var dto = new PatientRegisterDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                Password = "Password@123",
                Phone = "9999999999",
                DOB = DateTime.UtcNow.AddYears(-25)
            };

            var id = Guid.NewGuid();

            _mockService
                .Setup(s => s.RegisterPatientAsync(dto))
                .ReturnsAsync(id);

            var result = await _controller.PatientRegister(dto);

            var created = result as CreatedAtRouteResult;
            Assert.IsNotNull(created);
            Assert.AreEqual("GetPatientById", created.RouteName);
        }

        [TestMethod]
        public async Task GetByIdAsync_ExistingId_ReturnsOk()
        {
            var id = Guid.NewGuid();

            _mockService
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(new PatientResponseDto { Id = id });

            var result = await _controller.GetByIdAsync(id);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        public async Task UpdatePutAsync_ValidRequest_ReturnsNoContent()
        {
            var id = Guid.NewGuid();
            var dto = new PatientUpdateDto();
            _mockService.Setup(s => s.UpdateAsync(id, dto)).Returns(Task.CompletedTask);

            var result = await _controller.UpdatePutAsync(id, dto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task UpdatePatchAsync_ValidRequest_ReturnsNoContent()
        {
            var id = Guid.NewGuid();
            var dto = new PatientUpdateDto();
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