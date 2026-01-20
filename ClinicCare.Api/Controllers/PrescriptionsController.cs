using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Shared.DTOs.Prescription;
using ClinicCare.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicCare.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;

        public PrescriptionsController(
            IPrescriptionService prescriptionService,
            IDoctorService doctorService,
             IPatientService patientService)
        {
            _prescriptionService = prescriptionService;
            _doctorService = doctorService;
            _patientService = patientService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var prescriptions = await _prescriptionService.GetAllAsync();
            return Ok(prescriptions);
        }

        [HttpGet("{id}", Name = "GetPrescriptionById")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var prescription = await _prescriptionService.GetByIdAsync(id);
            return Ok(prescription);
        }

        [HttpGet("patient/{id}")]
        public async Task<IActionResult> GetByPatientIdAsync(Guid id)
        {
            var prescriptions = await _prescriptionService.GetByPatientIdAsync(id);
            return Ok(prescriptions);
        }

        [HttpGet("doctor/{id}")]
        public async Task<IActionResult> GetByDoctorIdAsync(Guid id)
        {
            //var doctor = await _doctorService.GetByIdAsync(id);
            //if (doctor is null || doctor.Role != EmployeeRole.Doctor)
            //    return BadRequest();

            var prescriptions = await _prescriptionService.GetByDoctorIdAsync(id);
            return Ok(prescriptions);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] PrescriptionCreateDto dto)
        {
            var id = await _prescriptionService.CreateAsync(dto);
            return CreatedAtRoute("GetPrescriptionById", new { id}, null);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            //    var prescription = await _prescriptionService.GetByIdAsync(id);
            //    if (prescription is null)
            //        return NotFound();

            await _prescriptionService.DeleteAsync(id);
            return NoContent(); ;
        }
    }
}
