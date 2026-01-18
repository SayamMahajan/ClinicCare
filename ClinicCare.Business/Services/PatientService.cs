using ClinicCare.Business.Services.Interfaces;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.DataAccess.Models;
using ClinicCare.Shared.DTOs.Patient;

namespace ClinicCare.Business.Services
{
    public class PatientService : IPatientService
    {
        private readonly IGenericRepository<Patient> _repo;

        public PatientService(IGenericRepository<Patient> repo)
        {
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

        public async Task<PatientResponseDto?> GetByIdAsync(int id)
        {
            var patient = await _repo.GetByIdAsync(id);
            if (patient is null) return null;

            return new PatientResponseDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.Email,
                Phone = patient.Phone
            };
        }

        public async Task UpdateAsync(int id, PatientUpdateDto dto)
        {
            var patient = await _repo.GetByIdAsync(id);
            if (patient is null) return;

            patient.FirstName = dto.FirstName;
            patient.LastName = dto.LastName;
            patient.Phone = dto.Phone;
            patient.Address = dto.Address;
            patient.Password = dto.Password;

            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var patient = await _repo.GetByIdAsync(id);
            if (patient is null) return;

            await _repo.Delete(id);
            await _repo.SaveChangesAsync();
        }
    }
}