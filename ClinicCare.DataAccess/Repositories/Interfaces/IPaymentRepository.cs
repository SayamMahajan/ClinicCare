using ClinicCare.DataAccess.Models;

namespace ClinicCare.DataAccess.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<Payment?> GetByIdAsync(Guid id);
        Task<IEnumerable<Payment>> GetPaymentsForDoctorAsync(Guid doctorId);
        Task<IEnumerable<Payment>> GetPaymentsForPatientAsync(Guid patientId);
    }
}
