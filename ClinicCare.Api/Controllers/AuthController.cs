using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicCare.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("patient/login")]
        public async Task<IActionResult> PatientLogin(LoginRequestDto dto)
        {
            var result = await _authService.LoginPatientAsync(dto);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("employee/login")]
        public async Task<IActionResult> EmployeeLogin(LoginRequestDto dto)
        {
            var result = await _authService.LoginEmployeeAsync(dto);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("patient/register")]
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
