using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Helpers;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.DTOs.Prescription;
using ClinicCare.Shared.DTOs.Specialization;
using ClinicCare.Shared.Enums;

namespace ClinicCare.Business.Services
{
    public class SpecializationService : ISpecializationService
    {
        private readonly ISpecializationRepository _specializationRepo;
        private readonly ICurrentUser _currentUser;

        public SpecializationService(ISpecializationRepository specializationRepo, ICurrentUser currentUser)
        {
            _specializationRepo = specializationRepo;
            _currentUser = currentUser;
        }

        public async Task<PaginatedResult<SpecializationResponseDto>> GetAllAsync(PaginationParams pagination)
        {
            var result = await _specializationRepo.GetAllAsync(pagination);
            return MapPaginatedResult(result);
        }

        public async Task<SpecializationResponseDto?> GetByIdAsync(Guid id)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));

            var specialization = await _specializationRepo.GetByIdAsync(id);
            if (specialization is null)
                throw new NotFoundException($"Specialization with id {id} not found.");

            return MapToDto(specialization);
        }

        public async Task<Guid> CreateAsync(SpecializationCreateDto dto)
        {
            ValidationHelper.NotNull(dto, "Specialization data is required.");

            dto.Type = NormalizationHelper.NormalizeKey(dto.Type);

            ValidationHelper.NotEmpty(dto.Type, "Specialization type is required.");

            if (_currentUser.Role != UserRole.Admin)
                throw new ForbiddenException("Only admins can create specializations");

            var exists = await _specializationRepo.GetByTypeAsync(dto.Type);

            ValidationHelper.MustBeUnique(
                exists is not null,
                $"Specialization '{dto.Type}' already exists.");

            var specialization = new Specialization
            {
                Id = Guid.NewGuid(),
                Type = dto.Type,
            };


            await _specializationRepo.InsertAsync(specialization);
            await _specializationRepo.SaveChangesAsync();

            return specialization.Id;
        }

        public async Task DeleteAsync(Guid id)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));

            if (_currentUser.Role != UserRole.Admin)
                throw new ForbiddenException("Only admins can create specializations");

            var specialization = await _specializationRepo.GetByIdAsync(id);
            if (specialization is null)
                throw new NotFoundException($"Specialization with id {id} not found.");

            await _specializationRepo.DeleteAsync(id);
            await _specializationRepo.SaveChangesAsync();
        }

        private PaginatedResult<SpecializationResponseDto> MapPaginatedResult(PaginatedResult<Specialization> result)
        {
            return new PaginatedResult<SpecializationResponseDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                HasPreviousPage = result.HasPreviousPage,
                HasNextPage = result.HasNextPage
            };
        }

        private static SpecializationResponseDto MapToDto(Specialization s)
        {
            return new SpecializationResponseDto
            {
                Id = s.Id,
                Type = s.Type,
            };
        }
    }
}