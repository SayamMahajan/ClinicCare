using ClinicCare.Shared.DTOs.Appointment;
using ClinicCare.Shared.Enums;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentResponseDto>> GetAllAsync(AppointmentStatus? status);
        Task<AppointmentResponseDto?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(AppointmentCreateDto dto);
        Task UpdateStatusAsync(Guid id, AppointmentStatus status);
        Task DeleteAsync(Guid id);
    }
}