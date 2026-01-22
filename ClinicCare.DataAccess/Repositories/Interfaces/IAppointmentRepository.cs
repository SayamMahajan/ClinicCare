using ClinicCare.DataAccess.Models;

namespace ClinicCare.DataAccess.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAllWithDetailsAsync();
        Task<Appointment?> GetByIdWithDetailsAsync(Guid id);
        Task<IEnumerable<Appointment>> GetByDoctorIdAsync(Guid doctorId);
        Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId);
    }
}
