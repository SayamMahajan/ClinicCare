using ClinicCare.Api.Middlewares;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Shared.DTOs.Auth;
using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.DTOs.Patient;
using ClinicCare.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicCare.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeesController(IEmployeeService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeLoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginAsync(LoginRequestDto dto)
        {
            var result = await _service.LoginAsync(dto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterAsync(EmployeeRegisterDto dto)
        {
            var id = await _service.RegisterAsync(dto);
            return CreatedAtRoute("GetEmployeeById", new { id }, null);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeeResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] EmployeeRole? role)
        {
            var result = await _service.GetAllAsync(role);
            return Ok(result);
        }

        [HttpGet("doctor-details")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllDoctorsAsync([FromQuery] Guid? specializationId)
        {
            var result = await _service.GetAllDoctorsAsync(specializationId);
            return Ok(result);
        }

        [HttpGet("{id}", Name = "GetEmployeeById")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] EmployeeUpdateDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
