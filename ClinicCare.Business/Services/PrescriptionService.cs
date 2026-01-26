using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Helpers;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Prescription;
using ClinicCare.Shared.DTOs.Specialization;
using ClinicCare.Shared.Enums;
using System.Text.Json;

namespace ClinicCare.Business.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IGenericRepository<Prescription> _repo;
        private readonly IGenericRepository<Patient> _patientRepo;
        private readonly ICurrentUser _currentUser;

        public PrescriptionService(
            IGenericRepository<Prescription> repo,
            IGenericRepository<Patient> patientRepo,
            ICurrentUser currentUser
            )
        {
            _repo = repo;
            _patientRepo = patientRepo;
            _currentUser = currentUser;
        }
        public async Task<IEnumerable<PrescriptionResponseDto>> GetAllAsync()
        {
            IEnumerable<Prescription> prescriptions = [];

            if (_currentUser.Role == UserRole.Doctor)
                prescriptions = await _repo.FindAsync(p => p.DoctorId == _currentUser.UserId);
            else if (_currentUser.Role == UserRole.Patient)
                prescriptions = await _repo.FindAsync(p => p.PatientId == _currentUser.UserId);
            else
                throw new ForbiddenException("You are not authorized.");

            var dtos = new List<PrescriptionResponseDto>();

            return prescriptions.Select(MapToDto);
        }
        public async Task<PrescriptionResponseDto?> GetByIdAsync(Guid id)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));

            var prescription = await _repo.GetByIdAsync(id);
            if (prescription == null)
                throw new NotFoundException($"Prescription with id {id} not found.");

            if (_currentUser.Role == UserRole.Patient && _currentUser.UserId != prescription.PatientId)
                throw new ForbiddenException("You are not authorized");

            if (_currentUser.Role == UserRole.Doctor && _currentUser.UserId != prescription.DoctorId)
                throw new ForbiddenException("You are not authorized");

            return MapToDto(prescription);
        }

        public async Task<Guid> CreateAsync(PrescriptionCreateDto dto)
        {
            ValidationHelper.NotNull(dto, "Prescription data is required.");

            ValidationHelper.GuidNotEmpty(dto.PatientId, "PatientId");
            ValidationHelper.GuidNotEmpty(dto.DoctorId, "DoctorId");

            if (_currentUser.Role != UserRole.Doctor)
                throw new ForbiddenException(
                    "Only doctors can create prescriptions.");

            if (_currentUser.UserId != dto.DoctorId)
                throw new ForbiddenException(
                    "You can only create prescriptions for yourself.");

            if (_patientRepo.GetByIdAsync(dto.PatientId) is null)
                throw new BadRequestException($"Patient with id {dto.PatientId} not found.");

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

        private static List<MedicationDto> Deserialize(string json)
        {
            return JsonSerializer.Deserialize<List<MedicationDto>>(json)
                ?? throw new BadRequestException(
                    "Invalid prescription data.");
        }

        private static PrescriptionResponseDto MapToDto(Prescription p)
        {
            return new PrescriptionResponseDto
            {
                Id = p.Id,
                PatientId = p.PatientId,
                DoctorId = p.DoctorId,
                Description = Deserialize(p.Description)
            };
        }
    }
}
