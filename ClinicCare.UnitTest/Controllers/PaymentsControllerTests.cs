using ClinicCare.Api.Controllers;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Shared.DTOs.Payment;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ClinicCare.UnitTest.Controllers
{
    [TestClass]
    public class PaymentsControllerTests
    {
        private readonly Mock<IPaymentService> _mockService;
        private readonly PaymentsController _controller;

        public PaymentsControllerTests()
        {
            _mockService = new Mock<IPaymentService>();
            _controller = new PaymentsController(_mockService.Object);
        }

        [TestMethod]
        public async Task GetAllAsync_ValidRequest_ReturnsOkWithPayments()
        {
            var payments = new List<PaymentResponseDto>
            {
                new() { Id = Guid.NewGuid(), Amount = 500 },
                new() { Id = Guid.NewGuid(), Amount = 1000 }
            };

            _mockService
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(payments);

            var result = await _controller.GetAllAsync();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var data = okResult.Value as IEnumerable<PaymentResponseDto>;
            Assert.IsNotNull(data);

            CollectionAssert.AreEquivalent(
                payments,
                new List<PaymentResponseDto>(data)
            );
        }

        [TestMethod]
        public async Task GetByIdAsync_ExistingId_ReturnsOkWithPayment()
        {
            var id = Guid.NewGuid();
            var payment = new PaymentResponseDto
            {
                Id = id,
                Amount = 750
            };

            _mockService
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(payment);

            var result = await _controller.GetByIdAsync(id);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var data = okResult.Value as PaymentResponseDto;
            Assert.IsNotNull(data);

            Assert.AreEqual(id, data.Id);
        }

        [TestMethod]
        public async Task CreateAsync_ValidDto_ReturnsCreatedAtRoute()
        {
            var dto = new PaymentCreateDto { Amount = 1200 };
            var newId = Guid.NewGuid();

            _mockService
                .Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(newId);

            var result = await _controller.CreateAsync(dto);

            var createdResult = result as CreatedAtRouteResult;
            Assert.IsNotNull(createdResult);

            Assert.AreEqual("GetPaymentById", createdResult.RouteName);

            Assert.IsNotNull(createdResult.RouteValues);
            Assert.AreEqual(newId, createdResult.RouteValues["id"]);
        }
    }
}