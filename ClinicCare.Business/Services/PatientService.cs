using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Patient;
using ClinicCare.Shared.Enums;

namespace ClinicCare.Business.Services
{
    public class PatientService : IPatientService
    {
        private readonly IGenericRepository<Patient> _repo;
        private readonly ICurrentUser _currentUser;

        public PatientService(IGenericRepository<Patient> repo, ICurrentUser currentUser)
        {
            _currentUser = currentUser;
            _repo = repo;
        }

        public async Task<IEnumerable<PatientResponseDto>> GetAllAsync()
        {
            var patients = await _repo.GetAllAsync();

            return patients.Select(p => new PatientResponseDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                Phone = p.Phone
            });
        }

        public async Task<PatientResponseDto?> GetByIdAsync(Guid id)
        {
            var patient = await _repo.GetByIdAsync(id);
            if (patient is null)
                throw new NotFoundException($"Patient with id {id} not found.");

            if (_currentUser.Role == UserRole.Patient && _currentUser.UserId != id)
                throw new ForbiddenException("You are not authorized");

            return new PatientResponseDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.Email,
                Phone = patient.Phone
            };
        }

        public async Task UpdateAsync(Guid id, PatientUpdateDto dto)
        {
            var patient = await _repo.GetByIdAsync(id);
            if (patient is null)
                throw new NotFoundException($"Patient with id {id} not found.");

            if (_currentUser.Role == UserRole.Patient && _currentUser.UserId != id)
                throw new ForbiddenException("You are not authorized");

            patient.FirstName = dto.FirstName;
            patient.LastName = dto.LastName;
            patient.Phone = dto.Phone;
            patient.Address = dto.Address;
            patient.Password = dto.Password;

            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var patient = await _repo.GetByIdAsync(id);
            if (patient is null)
                throw new NotFoundException($"Patient with id {id} not found.");

            if (_currentUser.Role == UserRole.Patient && _currentUser.UserId != id)
                throw new ForbiddenException("You are not authorized");

            await _repo.Delete(id);
            await _repo.SaveChangesAsync();
        }
    }
}