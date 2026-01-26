using ClinicCare.Api.Controllers;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Shared.DTOs.Prescription;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ClinicCare.UnitTest.Controllers
{
    [TestClass]
    public class PrescriptionsControllerTests
    {
        private readonly Mock<IPrescriptionService> _mockService;
        private readonly PrescriptionsController _controller;

        public PrescriptionsControllerTests()
        {
            _mockService = new Mock<IPrescriptionService>();
            _controller = new PrescriptionsController(_mockService.Object);
        }

        [TestMethod]
        public async Task GetAllAsync_ValidRequest_ReturnsOkWithPrescriptions()
        {
            var prescriptions = new List<PrescriptionResponseDto>
            {
                new() { Id = Guid.NewGuid() },
                new() { Id = Guid.NewGuid() }
            };

            _mockService
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(prescriptions);

            var result = await _controller.GetAllAsync();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var data = okResult.Value as IEnumerable<PrescriptionResponseDto>;
            Assert.IsNotNull(data);

            CollectionAssert.AreEquivalent(
                prescriptions,
                new List<PrescriptionResponseDto>(data)
            );
        }

        [TestMethod]
        public async Task GetByIdAsync_ExistingId_ReturnsOkWithPrescription()
        {
            var id = Guid.NewGuid();
            var prescription = new PrescriptionResponseDto { Id = id };

            _mockService
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(prescription);

            var result = await _controller.GetByIdAsync(id);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var data = okResult.Value as PrescriptionResponseDto;
            Assert.IsNotNull(data);

            Assert.AreEqual(id, data.Id);
        }

        [TestMethod]
        public async Task CreateAsync_ValidDto_ReturnsCreatedAtRoute()
        {
            var dto = new PrescriptionCreateDto();
            var newId = Guid.NewGuid();

            _mockService
                .Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(newId);

            var result = await _controller.CreateAsync(dto);

            var createdResult = result as CreatedAtRouteResult;
            Assert.IsNotNull(createdResult);

            Assert.AreEqual("GetPrescriptionById", createdResult.RouteName);
            Assert.IsNotNull(createdResult.RouteValues);
            Assert.AreEqual(newId, createdResult.RouteValues["id"]);
        }

        [TestMethod]
        public async Task DeleteAsync_ExistingId_ReturnsNoContent()
        {
            var id = Guid.NewGuid();

            _mockService
                .Setup(s => s.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteAsync(id);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}