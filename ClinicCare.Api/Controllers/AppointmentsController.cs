using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Shared.DTOs.Appointment;
using ClinicCare.Shared.DTOs.Enums;
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

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var appointments = await _appointmentService.GetAllAsync();
            return Ok(appointments);
        }

        [HttpGet("{id:int}", Name = "GetAppointmentById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            if (appointment is null)
                return NotFound();

            return Ok(appointment);
        }

        [HttpGet("doctor/{id:int}")]
        public async Task<IActionResult> GetByDoctorAsync(int id)
        {
            var doctor = await _doctorService.GetByIdAsync(id);
            if (doctor is null || doctor.Role != EmployeeRole.Doctor)
                return BadRequest();

            var appointments = await _appointmentService.GetByDoctorAsync(id);
            return Ok(appointments);
        }

        [HttpGet("patient/{id:int}")]
        public async Task<IActionResult> GetByPatientAsync(int id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if (patient is null)
                return BadRequest();

            var appointments = await _appointmentService.GetByPatientAsync(id);
            return Ok(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] AppointmentCreateDto dto)
        {
            var id = await _appointmentService.CreateAsync(dto);
            return CreatedAtRoute("GetAppointmentById", new { id }, null);
        }
        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateStatusAsync(int id, [FromQuery] AppointmentStatus status)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            if (appointment is null)
                return NotFound();

            await _appointmentService.UpdateStatusAsync(id, status);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var appoitment = await _appointmentService.GetByIdAsync(id);
            if (appoitment is null)
                return NotFound();

            await _appointmentService.DeleteAsync(id);
            return NoContent(); ;
        }
    }
}
