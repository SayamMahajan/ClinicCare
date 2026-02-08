using ClinicCare.Api.Middlewares;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.DTOs.Specialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicCare.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class SpecializationsController : ControllerBase
    {
        private readonly ISpecializationService _specializationService;

        public SpecializationsController(ISpecializationService specializationService)
        {
            _specializationService = specializationService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<SpecializationResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationParams pagination)
        {
            var payments = await _specializationService.GetAllAsync(pagination);
            return Ok(payments);
        }

        [HttpGet("{id}", Name = "GetSpecializationById")]
        [ProducesResponseType(typeof(SpecializationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var prescription = await _specializationService.GetByIdAsync(id);
            return Ok(prescription);
        }

        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateAsync([FromBody] SpecializationCreateDto dto)
        {
            var id = await _specializationService.CreateAsync(dto);
            return CreatedAtRoute("GetSpecializationById", new { id }, null);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _specializationService.DeleteAsync(id);
            return NoContent(); ;
        }
    }
}
