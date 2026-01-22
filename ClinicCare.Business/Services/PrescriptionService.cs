using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Prescription;
using ClinicCare.Shared.Enums;
using System.Text.Json;

namespace ClinicCare.Business.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IGenericRepository<Prescription> _repo;
        private readonly ICurrentUser _currentUser;

        public PrescriptionService(
            IGenericRepository<Prescription> repo, 
            ICurrentUser currentUser
            )
        {
            _repo = repo;
            _currentUser = currentUser;
        }
        public async Task<IEnumerable<PrescriptionResponseDto>> GetAllAsync()
        {
            IEnumerable<Prescription> prescriptions = [];

            if (_currentUser.Role == UserRole.Doctor)
                prescriptions = await _repo.FindAsync(p => p.DoctorId == _currentUser.UserId);
            else if(_currentUser.Role == UserRole.Patient)
                prescriptions = await _repo.FindAsync(p => p.PatientId == _currentUser.UserId);

            var dtos = new List<PrescriptionResponseDto>();

            foreach (var prescription in prescriptions)
            {
                var descriptionList = JsonSerializer.Deserialize<List<MedicationDto>>(prescription.Description)!;
                dtos.Add(new PrescriptionResponseDto
                {
                    Id = prescription.Id,
                    PatientId = prescription.PatientId,
                    DoctorId = prescription.DoctorId,
                    Description = descriptionList
                });
            }

            return dtos;
        }
        public async Task<PrescriptionResponseDto?> GetByIdAsync(Guid id)
        {
            var prescription = await _repo.GetByIdAsync(id);
            if (prescription == null)
                throw new NotFoundException($"Prescription with id {id} not found.");

            if (_currentUser.Role == UserRole.Patient && _currentUser.UserId != prescription.PatientId)
                throw new ForbiddenException("You are not authorized");

            if (_currentUser.Role == UserRole.Doctor && _currentUser.UserId != prescription.DoctorId)
                throw new ForbiddenException("You are not authorized");

            var descriptionList = JsonSerializer.Deserialize<List<MedicationDto>>(prescription.Description)!;

            return new PrescriptionResponseDto
            {
                Id = prescription.Id,
                PatientId = prescription.PatientId,
                DoctorId = prescription.DoctorId,
                Description = descriptionList,
            };
        }

        public async Task<Guid> CreateAsync(PrescriptionCreateDto dto)
        {
            var prescription = new Prescription
            {
                Id = Guid.NewGuid(),
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                Description = JsonSerializer.Serialize(dto.Description)
            };


            await _repo.InsertAsync(prescription);
            await _repo.SaveChangesAsync();

            return prescription.Id;
        }

        public async Task DeleteAsync(Guid id)
        {
            var prescription = await _repo.GetByIdAsync(id);
            if (prescription == null)
                throw new NotFoundException($"Prescription with id {id} not found.");

            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
        }
    }
}