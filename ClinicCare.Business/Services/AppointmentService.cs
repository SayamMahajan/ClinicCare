using ClinicCare.Business.Services.Interfaces;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Appointment;
using ClinicCare.Shared.DTOs.Enums;

namespace ClinicCare.Business.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IGenericRepository<Appointment> _repo;

        public AppointmentService(IGenericRepository<Appointment> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetAllAsync()
        {
            var appointments = await _repo.GetAllAsync();

            return appointments.Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                DoctorId = a.DoctorId,
                Date = a.Date,
                TimeSlot = a.TimeSlot,
                Status = a.Status
            });
        }

        public async Task<AppointmentResponseDto?> GetByIdAsync(int id)
        {
            var appointment = await _repo.GetByIdAsync(id);
            if (appointment is null) return null;

            return new AppointmentResponseDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                Date = appointment.Date,
                TimeSlot = appointment.TimeSlot,
                Status = appointment.Status
            };
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetByDoctorAsync(int doctorId)
        {
            var appointments = await _repo.FindAsync(a => a.DoctorId == doctorId);

            return appointments.Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                DoctorId = a.DoctorId,
                Date = a.Date,
                TimeSlot = a.TimeSlot,
                Status = a.Status
            });
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetByPatientAsync(int patientId)
        {
            var appointments = await _repo.FindAsync(a => a.PatientId == patientId);

            return appointments.Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                DoctorId = a.DoctorId,
                Date = a.Date,
                TimeSlot = a.TimeSlot,
                Status = a.Status
            });
        }

        public async Task<int> CreateAsync(AppointmentCreateDto dto)
        {
            var appointment = new Appointment
            {
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

        public async Task UpdateStatusAsync(int id, AppointmentStatus status)
        {
            var appt = await _repo.GetByIdAsync(id);
            if (appt is null) throw new Exception("Appointment not found");

            appt.Status = status;
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var appointment = await _repo.GetByIdAsync(id);
            if (appointment is null) return;

            await _repo.Delete(id);
            await _repo.SaveChangesAsync();
        }
    }
}