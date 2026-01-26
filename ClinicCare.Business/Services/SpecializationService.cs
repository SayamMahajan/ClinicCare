using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Helpers;
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

            return specializations.Select(MapToDto);
        }

        public async Task<SpecializationResponseDto?> GetByIdAsync(Guid id)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));

            var specialization = await _repo.GetByIdAsync(id);
            if (specialization is null)
                throw new NotFoundException($"Specialization with id {id} not found.");

            return MapToDto(specialization);
        }

        public async Task<Guid> CreateAsync(SpecializationCreateDto dto)
        {
            ValidationHelper.NotNull(dto, "Specialization data is required.");

            dto.Type = NormalizationHelper.NormalizeKey(dto.Type);

            ValidationHelper.NotEmpty(dto.Type, "Specialization type is required.");

            var exists = await _repo.FindAsync(s =>
                s.Type == dto.Type);

            ValidationHelper.MustBeUnique(
                exists.Any(),
                $"Specialization '{dto.Type}' already exists.");

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
            ValidationHelper.GuidNotEmpty(id, nameof(id));

            var specialization = await _repo.GetByIdAsync(id);
            if (specialization is null)
                throw new NotFoundException($"Specialization with id {id} not found.");

            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
        }

        private static SpecializationResponseDto MapToDto(DoctorSpecialization s)
        {
            return new SpecializationResponseDto
            {
                Id = s.Id,
                Type = s.Type,
            };
        }
    }
}