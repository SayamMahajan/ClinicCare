using ClinicCare.Api.Middlewares;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.DTOs.Pagination;
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
        public async Task<IActionResult> LoginAsync(EmployeeLoginDto dto)
        {
            var result = await _service.LoginAsync(dto);
            return Ok(result);
        }

        [AllowAnonymous]
        //[Authorize(Roles = "Admin")]
        [HttpPost("register")]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterAsync(EmployeeRegisterDto dto)
        {
            var id = await _service.RegisterAsync(dto);
            return CreatedAtRoute("GetEmployeeById", new { id }, null);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<EmployeeResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] EmployeeSearchParams searchParams)
        {
            var result = await _service.GetAllAsync(searchParams);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-dashboard")]
        [ProducesResponseType(typeof(AdminDashboardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAdminDashboardAsync()
        {
            var dashboard = await _service.GetAdminDashboardAsync();
            return Ok(dashboard);
        }

        [HttpGet("{id}", Name = "GetEmployeeById")]
        [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status200OK)]
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
        public async Task<IActionResult> UpdatePutAsync(Guid id, [FromBody] EmployeeUpdateDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePatchAsync(Guid id, [FromBody] EmployeeUpdateDto dto)
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
