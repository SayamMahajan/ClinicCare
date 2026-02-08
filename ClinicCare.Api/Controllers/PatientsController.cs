using ClinicCare.Api.Middlewares;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.DTOs.Patient;
using Microsoft.AspNetCore.Authorization;
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

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(IEnumerable<PatientLoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginPatientAsync(PatientLoginDto dto)
        {
            var result = await _patientService.LoginPatientAsync(dto);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatientRegister(PatientRegisterDto dto)
        {
            var patientId = await _patientService.RegisterPatientAsync(dto);
            return CreatedAtRoute(
                "GetPatientById",
                new { id = patientId },
                null
            );
        }

        [Authorize(Roles = "Admin, Doctor")]
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<PatientResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationParams pagination)
        {
            var patients = await _patientService.GetAllAsync(pagination);
            return Ok(patients);
        }

        [HttpGet("{id}", Name = "GetPatientById")]
        [ProducesResponseType(typeof(PatientResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            return Ok(patient);
        }

        [Authorize(Roles = "Patient")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePutAsync(Guid id, [FromBody] PatientUpdateDto dto)
        {
            await _patientService.UpdateAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Patient")]
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePatchAsync(Guid id, [FromBody] PatientUpdateDto dto)
        {
            await _patientService.UpdateAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Patient, Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _patientService.DeleteAsync(id);
            return NoContent(); ;
        }
    }
}
