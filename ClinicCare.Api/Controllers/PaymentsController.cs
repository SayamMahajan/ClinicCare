using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Shared.DTOs.Enums;
using ClinicCare.Shared.DTOs.Payment;
using Microsoft.AspNetCore.Mvc;

namespace ClinicCare.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;

        public PaymentsController(IPaymentService paymentService,
            IDoctorService doctorService,
            IPatientService patientService)
        {
            _paymentService = paymentService;
            _doctorService = doctorService;
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var payments = await _paymentService.GetAllAsync();
            return Ok(payments);
        }

        [HttpGet("{id:int}", Name = "GetPaymentById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment is null)
                return NotFound();

            return Ok(payment);
        }

        [HttpGet("recipient/{id:int}")]
        public async Task<IActionResult> GetByRecipientAsync(int id)
        {
            var recipient = await _doctorService.GetByIdAsync(id);
            if (recipient is null || recipient.Role != EmployeeRole.Doctor)
                return BadRequest();

            var payments = await _paymentService.GetByRecipientAsync(id);
            return Ok(payments);
        }

        [HttpGet("sender/{id:int}")]
        public async Task<IActionResult> GetBySenderAsync(int id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if (patient is null)
                return BadRequest();

            var payments = await _paymentService.GetBySenderAsync(id);
            return Ok(payments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] PaymentCreateDto dto)
        {
            var id = await _paymentService.CreateAsync(dto);
            return CreatedAtRoute("GetPaymentById", new { id }, null);
        }

        //[HttpDelete("{id:int}")]
        //public async Task<IActionResult> DeleteAsync(int id)
        //{
        //    var payment = await _paymentService.GetByIdAsync(id);
        //    if (payment is null)
        //        return NotFound();

        //    await _paymentService.DeleteAsync(id);
        //    return NoContent(); ;
        //}
    }
}
