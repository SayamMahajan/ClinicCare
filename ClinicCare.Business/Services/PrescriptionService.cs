using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Helpers;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.DTOs.Patient;
using ClinicCare.Shared.DTOs.Prescription;
using ClinicCare.Shared.Enums;
using System.Text.Json;

namespace ClinicCare.Business.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepo;
        private readonly IPatientRepository _patientRepo;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly ICurrentUser _currentUser;

        public PrescriptionService(
            IPrescriptionRepository prescriptionRepo,
            IPatientRepository patientRepo,
            IAppointmentRepository appointmentRepo,
            ICurrentUser currentUser
            )
        {
            _prescriptionRepo = prescriptionRepo;
            _patientRepo = patientRepo;
            _appointmentRepo = appointmentRepo;
            _currentUser = currentUser;
        }

        public async Task<PaginatedResult<PrescriptionResponseDto>> GetAllAsync(PrescriptionSearchParams searchParams)
        {
            PaginatedResult<Prescription> result = _currentUser.Role switch
            {
                UserRole.Doctor => await _prescriptionRepo.GetAllAsync(searchParams, patientId: null, doctorId: _currentUser.UserId),
                UserRole.Patient => await _prescriptionRepo.GetAllAsync(searchParams, patientId: _currentUser.UserId, doctorId: null),
                _ => throw new ForbiddenException("Only doctors and patients can view prescriptions.")
            };

            return MapPaginatedResult(result);
        }

        public async Task<PrescriptionResponseDto?> GetByIdAsync(Guid id)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));

            var prescription = await _prescriptionRepo.GetByIdAsync(id)
                ?? throw new NotFoundException($"Prescription with id {id} not found.");

            var appointment = prescription.Appointment;

            if (_currentUser.Role == UserRole.Patient &&
                _currentUser.UserId != appointment.PatientId)
                throw new ForbiddenException("You are not authorized");

            if (_currentUser.Role == UserRole.Doctor &&
                _currentUser.UserId != appointment.DoctorId)
                throw new ForbiddenException("You are not authorized");

            return MapToDto(prescription);
        }

        public async Task<Guid> CreateAsync(PrescriptionCreateDto dto)
        {
            ValidationHelper.NotNull(dto, "Prescription data is required.");
            ValidationHelper.GuidNotEmpty(dto.AppointmentId, nameof(dto.AppointmentId));

            if (_currentUser.Role != UserRole.Doctor)
                throw new ForbiddenException("Only doctors can create prescriptions.");

            var appointment = await _appointmentRepo.GetByIdAsync(dto.AppointmentId)
                ?? throw new BadRequestException($"Appointment {dto.AppointmentId} not found.");

            if (appointment.DoctorId != _currentUser.UserId)
                throw new ForbiddenException("You can only prescribe for your own appointments.");

            if (appointment.Prescription is not null)
                throw new BadRequestException("Prescription already exists for this appointment.");

            var prescription = new Prescription
            {
                Id = Guid.NewGuid(),
                AppointmentId = appointment.Id,
                Description = JsonSerializer.Serialize(dto.Description)
            };

            await _prescriptionRepo.InsertAsync(prescription);

            appointment.Prescription = prescription;

            await _prescriptionRepo.SaveChangesAsync();

            return prescription.Id;
        }

        private static List<MedicationDto> Deserialize(string json)
        {
            return JsonSerializer.Deserialize<List<MedicationDto>>(json)
                ?? throw new BadRequestException(
                    "Invalid prescription data.");
        }

        private PaginatedResult<PrescriptionResponseDto> MapPaginatedResult(PaginatedResult<Prescription> result)
        {
            return new PaginatedResult<PrescriptionResponseDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                HasPreviousPage = result.HasPreviousPage,
                HasNextPage = result.HasNextPage
            };
        }

        private static PrescriptionResponseDto MapToDto(Prescription p)
        {
            var appointment = p.Appointment;
            return new PrescriptionResponseDto
            {
                Id = p.Id,
                AppointmentId = p.AppointmentId,
                Patient = new PatientMiniDto
                {
                    Id = appointment.Patient.Id,
                    FirstName = appointment.Patient.FirstName,
                    LastName = appointment.Patient.LastName
                },
                Doctor = new DoctorMiniDto
                {
                    Id = appointment.Doctor.Id,
                    FirstName = appointment.Doctor.FirstName,
                    LastName = appointment.Doctor.LastName
                },
                Description = Deserialize(p.Description),
                CreatedAt = p.CreatedAt,
            };
        }
    }
}
