using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Helpers;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Appointment;
using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.DTOs.Patient;
using ClinicCare.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.Data;
namespace ClinicCare.Business.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IGenericRepository<Appointment> _repo;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IGenericRepository<Employee> _employeeRepo;
        private readonly ICurrentUser _currentUser;

        public AppointmentService(
            IGenericRepository<Appointment> repo,
            IAppointmentRepository appointmentRepo,
            IGenericRepository<Employee> employeeRepo,
            ICurrentUser currentUser
            )
        {
            _repo = repo;
            _appointmentRepo = appointmentRepo;
            _employeeRepo = employeeRepo;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetAllAsync(AppointmentStatus? status = null)
        {
            IEnumerable<Appointment> appointments = _currentUser.Role switch
            {
                UserRole.Admin => await _appointmentRepo.GetAllWithDetailsAsync(),
                UserRole.Doctor => await _appointmentRepo.GetByDoctorIdAsync(_currentUser.UserId),
                UserRole.Patient => await _appointmentRepo.GetByPatientIdAsync(_currentUser.UserId),
                _ => throw new ForbiddenException("Invalid role")
            };

            if (status.HasValue)
                appointments = appointments.Where(a => a.Status == status.Value);

            return appointments.Select(MapToDto);
        }


        public async Task<AppointmentResponseDto?> GetByIdAsync(Guid id)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));

            var appointment = await _appointmentRepo.GetByIdWithDetailsAsync(id);
            if (appointment is null)
                throw new NotFoundException($"Appointment with id {id} not found.");

            if (_currentUser.Role == UserRole.Patient && _currentUser.UserId != appointment.PatientId)
                throw new ForbiddenException($"You are not authorized");

            if (_currentUser.Role == UserRole.Doctor && _currentUser.UserId != appointment.DoctorId)
                throw new ForbiddenException($"You are not authorized");

            return MapToDto(appointment);
        }

        public async Task<Guid> CreateAsync(AppointmentCreateDto dto)
        {
            ValidationHelper.GuidNotEmpty(dto.PatientId, nameof(dto.PatientId));
            ValidationHelper.GuidNotEmpty(dto.DoctorId, nameof(dto.DoctorId));

            if (_currentUser.Role == UserRole.Patient &&
                _currentUser.UserId != dto.PatientId)
                throw new ForbiddenException("You can book only for yourself.");

            ValidationHelper.DateNotInPast(dto.Date, nameof(dto.Date));

            if (dto.Date < DateTime.UtcNow.AddHours(24))
                throw new BadRequestException(
                    "Appointments must be booked at least 24 hours in advance.");

            var doctor = await _employeeRepo.GetByIdAsync(dto.DoctorId)
                ?? throw new NotFoundException($"Doctor with Id{dto.DoctorId} not found.");

            if (doctor.Role != EmployeeRole.Doctor || doctor.DoctorDetails is null)
                throw new BadRequestException("Invalid doctor.");

            var specializationId =
                doctor.DoctorDetails.SpecializationId;

            var patientAppointments =
                await _appointmentRepo.GetByPatientIdAsync(dto.PatientId);

            var conflict = patientAppointments.Any(a =>
                a.Date == dto.Date &&
                a.TimeSlot == dto.TimeSlot &&
                a.Status != AppointmentStatus.Cancelled &&
                a.Doctor.DoctorDetails!.SpecializationId == specializationId);

            if (conflict)
                throw new ConflictException(
                    "You already have an appointment with this specialization in this slot.");

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                Date = dto.Date,
                TimeSlot = dto.TimeSlot,
                Status = AppointmentStatus.Requested
            };

            await _repo.InsertAsync(appointment);
            await _repo.SaveChangesAsync();

            return appointment.Id;
        }

        public async Task UpdateAsync(Guid id, AppointmentUpdateDto dto)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));
            ValidationHelper.NotNull(dto, "Appointment data is required.");

            var appointment = await _repo.GetByIdAsync(id);
            if (appointment is null) 
                throw new NotFoundException($"Appointment with Id{id} not found.");

            if(_currentUser.Role == UserRole.Doctor && appointment.DoctorId != _currentUser.UserId)
                throw new ForbiddenException("You are not authorized");

            if(dto.Status is not null && _currentUser.Role == UserRole.Doctor)
                appointment.Status = dto.Status.Value;
            
            if(dto.Date is not null)
                ValidationHelper.DateNotInPast(dto.Date.Value, nameof(dto.Date));
                appointment.Date = dto.Date!.Value;

            if(dto.TimeSlot is not null)
                appointment.TimeSlot = dto.TimeSlot!.Value;

            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));

            var appointment = await _repo.GetByIdAsync(id);
            if (appointment is null)
                throw new NotFoundException($"Appointment with Id{id} not found.");

            if(_currentUser.Role == UserRole.Patient && appointment.PatientId != _currentUser.UserId && appointment.Status != AppointmentStatus.Completed)
                throw new ForbiddenException("You are not authorized");

            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
        }

        private static AppointmentResponseDto MapToDto(Appointment a)
        {
            return new AppointmentResponseDto
            {
                Id = a.Id,
                Date = a.Date,
                TimeSlot = a.TimeSlot,
                Status = a.Status,
                Patient = new PatientMiniDto
                {
                    Id = a.Patient.Id,
                    FirstName = a.Patient.FirstName,
                    LastName = a.Patient.LastName
                },
                Doctor = new DoctorMiniDto
                {
                    Id = a.Doctor.Id,
                    FirstName = a.Doctor.FirstName,
                    LastName = a.Doctor.LastName
                }
            };
        }
    }
}