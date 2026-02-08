using ClinicCare.Shared.DTOs.Appointment;
using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.Enums;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<PaginatedResult<AppointmentResponseDto>> GetAllAsync(AppointmentSearchParams searchParams);
        Task<AppointmentResponseDto?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(AppointmentCreateDto dto);
        Task UpdateAsync(Guid id, AppointmentUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}