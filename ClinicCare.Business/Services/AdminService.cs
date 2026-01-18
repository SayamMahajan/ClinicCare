using ClinicCare.Business.Services.Interfaces;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Admin;
using ClinicCare.Shared.DTOs.Enums;
using System.Data;

namespace ClinicCare.Business.Services
{
    public class AdminService : IAdminService
    {
        private readonly IGenericRepository<Employee> _repo;

        public AdminService(IGenericRepository<Employee> Repo)
        {
            _repo = Repo;
        }

        public async Task<IEnumerable<AdminResponseDto>> GetAllAsync()
        {
            var admins = await _repo.FindAsync(a => a.Role == EmployeeRole.Admin);

            return admins.Select(a => new AdminResponseDto
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email,
                Role = a.Role,
                DateOfJoining = a.DateOfJoining,
            });
        }

        public async Task<AdminResponseDto?> GetByIdAsync(int id)
        {
            var admin = await _repo.GetByIdAsync(id);
            if (admin is null || admin.Role != EmployeeRole.Admin) return null;

            return new AdminResponseDto
            {
                Id = admin.Id,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Email = admin.Email,
                Role = admin.Role,
                DateOfJoining = admin.DateOfJoining,
            };
        }

        public async Task UpdateAsync(int id, AdminUpdateDto dto)
        {
            var admin = await _repo.GetByIdAsync(id);
            if (admin is null || admin.Role != EmployeeRole.Admin) return;

            admin.FirstName = dto.FirstName;
            admin.LastName = dto.LastName;
            admin.Password = dto.Password;

            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var admin = await _repo.GetByIdAsync(id);
            if (admin is null || admin.Role != EmployeeRole.Admin) return;

            await _repo.Delete(id);
            await _repo.SaveChangesAsync();
        }
    }
}