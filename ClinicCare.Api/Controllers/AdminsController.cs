using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Shared.DTOs.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicCare.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminsController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var admins = await _adminService.GetAllAsync();
            return Ok(admins);
        }

        [HttpGet("{id:int}", Name = "GetAdminById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var admin = await _adminService.GetByIdAsync(id);
            if (admin is null)
                return NotFound();

            return Ok(admin);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] AdminUpdateDto dto)
        {
            var admin = await _adminService.GetByIdAsync(id);
            if (admin is null)
                return NotFound();

            await _adminService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var admin = await _adminService.GetByIdAsync(id);
            if (admin is null)
                return NotFound();

            await _adminService.DeleteAsync(id);
            return NoContent();
        }
    }
}
