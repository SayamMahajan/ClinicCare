using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Shared.DTOs.Patient;
using Microsoft.AspNetCore.Mvc;

namespace ClinicCare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var patients = await _patientService.GetAllAsync();
            return Ok(patients);
        }

        [HttpGet("{id:int}", Name = "GetPatientById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if (patient is null)
                return NotFound();

            return Ok(patient);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] PatientUpdateDto dto)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if (patient is null)
                return NotFound();

            await _patientService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if (patient is null)
                return NotFound();

            await _patientService.DeleteAsync(id);
            return NoContent(); ;
        }
    }
}
