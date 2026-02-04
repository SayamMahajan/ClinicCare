using ClinicCare.Shared.DTOs.Appointment;
using ClinicCare.Shared.Enums;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentResponseDto>> GetAllAsync(AppointmentStatus? status,
            Guid? prescriptionId);
        Task<AppointmentResponseDto?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(AppointmentCreateDto dto);
        Task UpdateAsync(Guid id, AppointmentUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}