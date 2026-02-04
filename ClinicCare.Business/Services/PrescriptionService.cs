using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Helpers;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.DTOs.Patient;
using ClinicCare.Shared.DTOs.Prescription;
using ClinicCare.Shared.Enums;
using System.Text.Json;

namespace ClinicCare.Business.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IGenericRepository<Prescription> _repo;
        private readonly IPrescriptionRepository _prescriptionRepo;
        private readonly IGenericRepository<Patient> _patientRepo;
        private readonly IGenericRepository<Appointment> _appointmentRepo;
        private readonly ICurrentUser _currentUser;

        public PrescriptionService(
            IGenericRepository<Prescription> repo,
            IPrescriptionRepository prescriptionRepo,
            IGenericRepository<Patient> patientRepo,
            IGenericRepository<Appointment> appointmentRepo,
            ICurrentUser currentUser
            )
        {
            _repo = repo;
            _prescriptionRepo = prescriptionRepo;
            _patientRepo = patientRepo;
            _appointmentRepo = appointmentRepo;
            _currentUser = currentUser;
        }
        public async Task<IEnumerable<PrescriptionResponseDto>> GetAllAsync()
        {
            IEnumerable<Prescription> prescriptions = [];

            if (_currentUser.Role == UserRole.Doctor)
                prescriptions = await _prescriptionRepo.GetPrescriptionsForDoctorAsync(_currentUser.UserId);
            else if (_currentUser.Role == UserRole.Patient)
                prescriptions = await _prescriptionRepo.GetPrescriptionsForPatientAsync(_currentUser.UserId);
            else
                throw new ForbiddenException("You are not authorized.");

            var dtos = new List<PrescriptionResponseDto>();

            return prescriptions.Select(MapToDto);
        }
        public async Task<PrescriptionResponseDto?> GetByIdAsync(Guid id)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));

            var prescription = await _prescriptionRepo.GetByIdAsync(id);
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
            ValidationHelper.GuidNotEmpty(dto.AppointmentId, "AppointmentId");

            var appointment = await _appointmentRepo.GetByIdAsync(dto.AppointmentId);
            if (appointment is null)
                throw new BadRequestException($"Appointment {dto.AppointmentId} not found.");

            if(appointment.PrescriptionId is not null)
                throw new BadRequestException($"Appointment {dto.AppointmentId} already had prescription.");

            if (_currentUser.Role != UserRole.Doctor)
                throw new ForbiddenException(
                    "Only doctors can create prescriptions.");

            if (_currentUser.UserId != dto.DoctorId)
                throw new ForbiddenException(
                    "You can only create prescriptions for yourself.");

            var patient = await _patientRepo.GetByIdAsync(dto.PatientId);
            if (patient is null)
                throw new BadRequestException($"Patient with id {dto.PatientId} not found.");

            var prescription = new Prescription
            {
                Id = Guid.NewGuid(),
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                Description = JsonSerializer.Serialize(dto.Description)
            };


            await _repo.InsertAsync(prescription);
            appointment.PrescriptionId = prescription.Id;
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
                Patient = new PatientMiniDto
                {
                    Id = p.Patient.Id,
                    FirstName = p.Patient.FirstName,
                    LastName = p.Patient.LastName
                },
                Doctor = new DoctorMiniDto
                {
                    Id = p.Doctor.Id,
                    FirstName = p.Doctor.FirstName,
                    LastName = p.Doctor.LastName
                },
                Description = Deserialize(p.Description)
            };
        }
    }
}
