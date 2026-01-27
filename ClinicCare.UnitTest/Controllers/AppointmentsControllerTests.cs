using ClinicCare.Api.Controllers;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Shared.DTOs.Appointment;
using ClinicCare.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ClinicCare.UnitTest.Controllers
{
    [TestClass]
    public class AppointmentsControllerTests
    {
        private readonly Mock<IAppointmentService> _mockService;
        private readonly AppointmentsController _controller;

        public AppointmentsControllerTests()
        {
            _mockService = new Mock<IAppointmentService>();
            _controller = new AppointmentsController(_mockService.Object);
        }

        [TestMethod]
        public async Task GetAllAsync_ValidRequest_ReturnsOk()
        {
            var appointments = new List<AppointmentResponseDto>
            {
                new() { Id = Guid.NewGuid(), Status = AppointmentStatus.Requested }
            };

            _mockService
                .Setup(s => s.GetAllAsync(null))
                .ReturnsAsync(appointments);

            var result = await _controller.GetAllAsync(null);

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);

            var data = ok.Value as IEnumerable<AppointmentResponseDto>;
            Assert.IsNotNull(data);
            Assert.AreEqual(1, data.Count());
        }

        [TestMethod]
        public async Task GetByIdAsync_ExistingId_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var dto = new AppointmentResponseDto { Id = id };

            _mockService
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(dto);

            var result = await _controller.GetByIdAsync(id);

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);

            var data = ok.Value as AppointmentResponseDto;
            Assert.AreEqual(id, data!.Id);
        }

        [TestMethod]
        public async Task CreateAsync_ValidDto_ReturnsCreatedAtRoute()
        {
            var dto = new AppointmentCreateDto
            {
                PatientId = Guid.NewGuid(),
                DoctorId = Guid.NewGuid(),
                Date = DateTime.UtcNow.AddDays(2),
                TimeSlot = TimeSlotType.Morning
            };

            var id = Guid.NewGuid();

            _mockService
                .Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(id);

            var result = await _controller.CreateAsync(dto);

            var created = result as CreatedAtRouteResult;
            Assert.IsNotNull(created);
            Assert.AreEqual("GetAppointmentById", created.RouteName);
            Assert.AreEqual(id, created.RouteValues!["id"]);
        }

        [TestMethod]
        public async Task UpdatePutAsync_ValidRequest_ReturnsNoContent()
        {
            var id = Guid.NewGuid();
            var dto = new AppointmentUpdateDto();
            _mockService.Setup(s => s.UpdateAsync(id, dto)).Returns(Task.CompletedTask);

            var result = await _controller.UpdatePutAsync(id, dto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task UpdatePatchAsync_ValidRequest_ReturnsNoContent()
        {
            var id = Guid.NewGuid();
            var dto = new AppointmentUpdateDto();
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