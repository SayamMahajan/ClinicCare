using ClinicCare.Shared.DTOs.Appointment;
using ClinicCare.Shared.DTOs.Enums;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentResponseDto>> GetAllAsync();
        Task<AppointmentResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<AppointmentResponseDto>> GetByDoctorAsync(int doctorId);
        Task<IEnumerable<AppointmentResponseDto>> GetByPatientAsync(int patientId);
        Task<int> CreateAsync(AppointmentCreateDto dto);
        Task UpdateStatusAsync(int id, AppointmentStatus status);
        Task DeleteAsync(int id);
    }
}