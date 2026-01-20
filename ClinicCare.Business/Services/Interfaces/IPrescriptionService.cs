using ClinicCare.Shared.DTOs.Prescription;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IPrescriptionService
    {
        Task<PrescriptionResponseDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<PrescriptionResponseDto>> GetByPatientIdAsync(Guid patientId);
        Task<IEnumerable<PrescriptionResponseDto>> GetByDoctorIdAsync(Guid doctorId);
        Task<Guid> CreateAsync(PrescriptionCreateDto dto);
        Task DeleteAsync(Guid id);
    }
}