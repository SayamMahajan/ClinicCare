using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Specialization;

namespace ClinicCare.Business.Services
{
    public class SpecializationService : ISpecializationService
    {
        private readonly IGenericRepository<DoctorSpecialization> _repo;

        public SpecializationService(IGenericRepository<DoctorSpecialization> repo, ICurrentUser currentUser)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<SpecializationResponseDto>> GetAllAsync()
        {
            var specializations = await _repo.GetAllAsync();

            return specializations.Select(p => new SpecializationResponseDto
            {
                Id = p.Id,
                Type = p.Type,
            });
        }

        public async Task<SpecializationResponseDto?> GetByIdAsync(Guid id)
        {
            var specialization = await _repo.GetByIdAsync(id);
            if (specialization is null)
                throw new NotFoundException($"Specialization with id {id} not found.");

            return new SpecializationResponseDto
            {
                Id = specialization.Id,
                Type = specialization.Type,
            };
        }

        public async Task<Guid> CreateAsync(SpecializationCreateDto dto)
        {
            var specialization = new DoctorSpecialization
            {
                Id = Guid.NewGuid(),
                Type = dto.Type,
            };


            await _repo.InsertAsync(specialization);
            await _repo.SaveChangesAsync();

            return specialization.Id;
        }

        public async Task DeleteAsync(Guid id)
        {
            var specialization = await _repo.GetByIdAsync(id);
            if (specialization is null)
                throw new NotFoundException($"Specialization with id {id} not found.");

            await _repo.Delete(id);
            await _repo.SaveChangesAsync();
        }
    }
}