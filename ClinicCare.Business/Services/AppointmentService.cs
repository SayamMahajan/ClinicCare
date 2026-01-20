using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Appointment;
using ClinicCare.Shared.Enums;
using System.Data;
namespace ClinicCare.Business.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IGenericRepository<Appointment> _repo;
        private readonly IGenericRepository<Employee> _employeeRepo;
        private readonly IGenericRepository<Patient> _patientRepo;
        private readonly ICurrentUser _currentUser;

        public AppointmentService(
            IGenericRepository<Appointment> repo, 
            ICurrentUser currentUser, 
            IGenericRepository<Employee> employeeRepo,
            IGenericRepository<Patient> patientRepo
            )
        {
            _repo = repo;
            _currentUser = currentUser;
            _employeeRepo = employeeRepo;
            _patientRepo = patientRepo;
        }

        public async Task<IEnumerable<AuthResponseDto>> GetAllAsync()
        {
            var appointments = await _repo.GetAllAsync();

            return appointments.Select(a => new AuthResponseDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                DoctorId = a.DoctorId,
                Date = a.Date,
                TimeSlot = a.TimeSlot,
                Status = a.Status
            });
        }

        public async Task<AuthResponseDto?> GetByIdAsync(Guid id)
        {
            var appointment = await _repo.GetByIdAsync(id);
            if (appointment is null)
                throw new NotFoundException($"Appointment with id {id} not found.");

            if (_currentUser.Role == UserRole.Patient && _currentUser.UserId != appointment.PatientId)
                throw new ForbiddenException($"You are not authorized");

            if (_currentUser.Role == UserRole.Doctor && _currentUser.UserId != appointment.DoctorId)
                throw new ForbiddenException($"You are not authorized");

            return new AuthResponseDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                Date = appointment.Date,
                TimeSlot = appointment.TimeSlot,
                Status = appointment.Status
            };
        }

        public async Task<IEnumerable<AuthResponseDto>> GetByStatusAsync(AppointmentStatus status)
        {
            IEnumerable<Appointment> appointments;

            if(_currentUser.Role == UserRole.Admin)
                 appointments = await _repo.FindAsync(a => a.Status == status);
            if (_currentUser.Role == UserRole.Doctor)
                appointments = await _repo.FindAsync(a => a.DoctorId == _currentUser.UserId && a.Status == status);
            else
                appointments = await _repo.FindAsync(a => a.PatientId == _currentUser.UserId && a.Status == status);

            return appointments.Select(a => new AuthResponseDto
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    DoctorId = a.DoctorId,
                    Date = a.Date,
                    TimeSlot = a.TimeSlot,
                    Status = a.Status
                });
        }

        public async Task<IEnumerable<AuthResponseDto>> GetByDoctorAsync(Guid doctorId)
        {
            var doctor = _employeeRepo.GetByIdAsync(doctorId);
            if (doctor is null)
                throw new BadRequestException($"Doctor with Id{doctorId} not available");

            IEnumerable<Appointment> appointments;

            if (_currentUser.Role == UserRole.Patient)
                appointments = await _repo.FindAsync(a => a.DoctorId == doctorId && a.PatientId == _currentUser.UserId);
            else
                appointments = await _repo.FindAsync(a => a.DoctorId == doctorId);

                return appointments.Select(a => new AuthResponseDto
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    DoctorId = a.DoctorId,
                    Date = a.Date,
                    TimeSlot = a.TimeSlot,
                    Status = a.Status
                });
        }

        public async Task<IEnumerable<AuthResponseDto>> GetByPatientAsync(Guid patientId)
        {
            var patient = _patientRepo.GetByIdAsync(patientId);
            if (patient is null)
                throw new BadRequestException($"Patient with Id{patientId} not available");

            if (_currentUser.Role == UserRole.Patient && _currentUser.UserId != patientId)
                throw new ForbiddenException("You are not authorized");

            var appointments = await _repo.FindAsync(a => a.PatientId == patientId);

            return appointments.Select(a => new AuthResponseDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                DoctorId = a.DoctorId,
                Date = a.Date,
                TimeSlot = a.TimeSlot,
                Status = a.Status
            });
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

            await _repo.Delete(id);
            await _repo.SaveChangesAsync();
        }
    }
}