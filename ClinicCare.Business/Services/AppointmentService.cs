using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Appointment;
using ClinicCare.Shared.Enums;
using System.Data;
using ClinicCare.Shared.DTOs.Patient;
using ClinicCare.Shared.DTOs.Doctor;
namespace ClinicCare.Business.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IGenericRepository<Appointment> _repo;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly ICurrentUser _currentUser;

        public AppointmentService(
            IGenericRepository<Appointment> repo,
            IAppointmentRepository appointmentRepo,
            ICurrentUser currentUser
            )
        {
            _repo = repo;
            _appointmentRepo = appointmentRepo;
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

            return appointments.Select(a => new AppointmentResponseDto
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
            });
        }


        public async Task<AppointmentResponseDto?> GetByIdAsync(Guid id)
        {
            var appointment = await _appointmentRepo.GetByIdWithDetailsAsync(id);
            if (appointment is null)
                throw new NotFoundException($"Appointment with id {id} not found.");

            if (_currentUser.Role == UserRole.Patient && _currentUser.UserId != appointment.PatientId)
                throw new ForbiddenException($"You are not authorized");

            if (_currentUser.Role == UserRole.Doctor && _currentUser.UserId != appointment.DoctorId)
                throw new ForbiddenException($"You are not authorized");

            return new AppointmentResponseDto
            {
                Id = appointment.Id,
                Date = appointment.Date,
                TimeSlot = appointment.TimeSlot,
                Status = appointment.Status,
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
                }
            };
        }

        public async Task<Guid> CreateAsync(AppointmentCreateDto dto)
        {
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

        public async Task UpdateStatusAsync(Guid id, AppointmentStatus status)
        {
            var appointment = await _repo.GetByIdAsync(id);
            if (appointment is null) 
                throw new NotFoundException($"Appointment with Id{id} not found.");

            if(_currentUser.Role == UserRole.Doctor && appointment.DoctorId != _currentUser.UserId)
                throw new ForbiddenException("You are not authorized");

            appointment.Status = status;
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var appointment = await _repo.GetByIdAsync(id);
            if (appointment is null)
                throw new NotFoundException($"Appointment with Id{id} not found.");

            if(_currentUser.Role == UserRole.Patient && appointment.PatientId != _currentUser.UserId && appointment.Status != AppointmentStatus.Requested)
                throw new ForbiddenException("You are not authorized");

            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
        }
    }
}