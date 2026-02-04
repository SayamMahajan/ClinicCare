using ClinicCare.DataAccess.Models;

namespace ClinicCare.DataAccess.Repositories.Interfaces
{
    public interface IPrescriptionRepository
    {
        Task<Prescription?> GetByIdAsync(Guid id);
        Task<IEnumerable<Prescription>> GetPrescriptionsForDoctorAsync(Guid doctorId);
        Task<IEnumerable<Prescription>> GetPrescriptionsForPatientAsync(Guid patientId);
    }
}
