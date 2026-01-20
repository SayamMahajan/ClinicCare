using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Shared.DTOs.Appointment;
using ClinicCare.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicCare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;

        public AppointmentsController(IAppointmentService appointmentService,
            IDoctorService doctorService,
            IPatientService patientService)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
            _patientService = patientService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var appointments = await _appointmentService.GetAllAsync();
            return Ok(appointments);
        }

        [HttpGet("{id}", Name = "GetAppointmentById")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            return Ok(appointment);
        }

        [HttpGet("doctor/{id}")]
        public async Task<IActionResult> GetByDoctorAsync(Guid id)
        {
            var appointments = await _appointmentService.GetByDoctorAsync(id);
            return Ok(appointments);
        }

        [HttpGet("patient/{id}")]
        public async Task<IActionResult> GetByPatientAsync(Guid id)
        {
            var appointments = await _appointmentService.GetByPatientAsync(id);
            return Ok(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] AppointmentCreateDto dto)
        {
            var id = await _appointmentService.CreateAsync(dto);
            return CreatedAtRoute("GetAppointmentById", new { id }, null);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatusAsync(Guid id, [FromQuery] AppointmentStatus status)
        {
            await _appointmentService.UpdateStatusAsync(id, status);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _appointmentService.DeleteAsync(id);
            return NoContent(); ;
        }
    }
}
