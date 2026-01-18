using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Shared.DTOs.Doctor;
using Microsoft.AspNetCore.Mvc;

namespace ClinicCare.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var doctors = await _doctorService.GetAllAsync();
            return Ok(doctors);
        }

        [HttpGet("{id:int}", Name = "GetDoctorById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var doctor = await _doctorService.GetByIdAsync(id);
            if (doctor is null)
                return NotFound();

            return Ok(doctor);
        }

        [HttpGet("specialist")]
        public async Task<IActionResult> GetBySpecialistTypeAsync([FromQuery] string type)
        {
            var doctors = await _doctorService.GetBySpecialistTypeAsync(type);
            return Ok(doctors);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] DoctorUpdateDto dto)
        {
            var doctor = await _doctorService.GetByIdAsync(id);
            if (doctor is null)
                return NotFound();

            await _doctorService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var doctor = await _doctorService.GetByIdAsync(id);
            if (doctor is null)
                return NotFound();

            await _doctorService.DeleteAsync(id);
            return NoContent();
        }
    }
}
