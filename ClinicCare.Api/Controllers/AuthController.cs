using ClinicCare.Api.Middlewares;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Shared.DTOs.Admin;
using ClinicCare.Shared.DTOs.Appointment;
using ClinicCare.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicCare.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("patient/login")]
        [ProducesResponseType(typeof(IEnumerable<PatientAuthResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatientLogin(LoginRequestDto dto)
        {
            var result = await _authService.LoginPatientAsync(dto);
            return Ok(result);
        }

        [HttpPost("employee/login")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeAuthResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EmployeeLogin(LoginRequestDto dto)
        {
            var result = await _authService.LoginEmployeeAsync(dto);
            return Ok(result);
        }
        
        [HttpPost("patient/register")]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatientRegister(PatientRegisterDto dto)
        {
            var patientId = await _authService.RegisterPatientAsync(dto);
            return CreatedAtRoute(
                "GetPatientById",
                new { id = patientId },
                null
            );
        }

        [HttpPost("doctor/register")]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DoctorRegister(DoctorRegisterDto dto)
        {
            var doctorId = await _authService.RegisterDoctorAsync(dto);
            return CreatedAtRoute(
                "GetDoctorById",
                new { id = doctorId },
                null
            );
        }

        [HttpPost("admin/register")]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AdminRegister(AdminRegisterDto dto)
        {
            var adminId = await _authService.RegisterAdminAsync(dto);
            return CreatedAtRoute(
                "GetAdminById",
                new { id = adminId },
                null
            );
        }
    }
}
