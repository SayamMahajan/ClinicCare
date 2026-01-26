using ClinicCare.Api.Controllers;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Shared.DTOs.Specialization;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ClinicCare.UnitTest.Controllers
{
    [TestClass]
    public class SpecializationsControllerTests
    {
        private readonly Mock<ISpecializationService> _mockService;
        private readonly SpecializationsController _controller;

        public SpecializationsControllerTests()
        {
            _mockService = new Mock<ISpecializationService>();
            // The .Object property returns the actual IOrderService proxy from the mock
            _controller = new SpecializationsController(_mockService.Object);
        }

        [TestMethod]
        public async Task GetAllAsync_ValidRequest_ReturnsOkWithSpecializations()
        {
            var specializations = new List<SpecializationResponseDto>
            {
                new() { Id = Guid.NewGuid(), Type = "Cardiologist" },
                new() { Id = Guid.NewGuid(), Type = "ENT" }
            };

            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(specializations);

            var result = await _controller.GetAllAsync();

            Assert.IsNotNull(result);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var data = okResult.Value as IEnumerable<SpecializationResponseDto>;
            Assert.IsNotNull(data);

            CollectionAssert.AreEquivalent(specializations, new List<SpecializationResponseDto>(data));
        }

        [TestMethod]
        public async Task GetSpecialization_ExistingId_ReturnsOkWithSpecialization()
        {
            var specializationId = Guid.NewGuid(); 
            var specializationResponse = new SpecializationResponseDto { Id = specializationId, Type = "Cardiologist" };

            _mockService.Setup(s => s.GetByIdAsync(specializationId))
                             .ReturnsAsync(specializationResponse);

            var result = await _controller.GetByIdAsync(specializationId);

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);

            var data = ok.Value as SpecializationResponseDto;
            Assert.IsNotNull(data);

            Assert.AreEqual(specializationId, data.Id);
        }

        [TestMethod]
        public async Task CreateAsync_ValidDto_ReturnsCreatedAtRoute()
        {
            var dto = new SpecializationCreateDto { Type = "Psychiatrist" };
            var newId = Guid.NewGuid();
            _mockService.Setup(s => s.CreateAsync(dto)).ReturnsAsync(newId);

            var result = await _controller.CreateAsync(dto);

            Assert.IsNotNull(result);

            var createdResult = result as CreatedAtRouteResult;
            Assert.IsNotNull(createdResult);

            Assert.AreEqual("GetSpecializationById", createdResult.RouteName);

            Assert.IsNotNull(createdResult.RouteValues);
            Assert.AreEqual(newId, createdResult.RouteValues["id"]);
        }

        [TestMethod]
        public async Task DeleteAsync_ExistingId_ReturnsNoContent()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteAsync(id);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}