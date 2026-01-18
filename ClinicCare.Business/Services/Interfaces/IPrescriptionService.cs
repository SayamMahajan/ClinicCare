using ClinicCare.Shared.DTOs.Prescription;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IPrescriptionService
    {
        Task<IEnumerable<PrescriptionResponseDto>> GetAllAsync();
        Task<PrescriptionResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<PrescriptionResponseDto>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<PrescriptionResponseDto>> GetByDoctorIdAsync(int doctorId);
        Task<int> CreateAsync(PrescriptionCreateDto dto);
        Task DeleteAsync(int id);
    }
}