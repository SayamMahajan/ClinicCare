using ClinicCare.Shared.DTOs.Appointment;
using ClinicCare.Shared.Enums;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AuthResponseDto>> GetAllAsync();
        Task<AuthResponseDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<AuthResponseDto>> GetByStatusAsync(AppointmentStatus status);
        Task<IEnumerable<AuthResponseDto>> GetByDoctorAsync(Guid doctorId);
        Task<IEnumerable<AuthResponseDto>> GetByPatientAsync(Guid patientId);
        Task<Guid> CreateAsync(AppointmentCreateDto dto);
        Task UpdateStatusAsync(Guid id, AppointmentStatus status);
        Task DeleteAsync(Guid id);
    }
}