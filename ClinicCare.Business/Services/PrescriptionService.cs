using ClinicCare.Business.Services.Interfaces;
using System.Text.Json;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Prescription;

namespace ClinicCare.Business.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IGenericRepository<Prescription> _repo;

        public PrescriptionService(IGenericRepository<Prescription> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<PrescriptionResponseDto>> GetAllAsync()
        {
            var prescriptions = await _repo.GetAllAsync();
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
            if (prescription == null) return null;

            var descriptionList = JsonSerializer.Deserialize<List<MedicationDto>>(prescription.Description)!;

            return new PrescriptionResponseDto
            {
                Id = prescription.Id,
                PatientId = prescription.PatientId,
                DoctorId = prescription.DoctorId,
                Description = descriptionList,
            };
        }

        public async Task<IEnumerable<PrescriptionResponseDto>> GetByPatientIdAsync(Guid patientId)
        {
            var prescriptions = await _repo.FindAsync(p => p.PatientId == patientId);
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

        public async Task<IEnumerable<PrescriptionResponseDto>> GetByDoctorIdAsync(Guid doctorId)
        {
            var prescriptions = await _repo.FindAsync(p => p.DoctorId == doctorId);
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
            if (prescription == null) return;

            await _repo.Delete(id);
            await _repo.SaveChangesAsync();
        }
    }
}