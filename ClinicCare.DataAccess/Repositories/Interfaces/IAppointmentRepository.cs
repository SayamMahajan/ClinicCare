using ClinicCare.DataAccess.Models;
using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.Enums;

namespace ClinicCare.DataAccess.Repositories.Interfaces
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<PaginatedResult<Appointment>> GetAllAsync(AppointmentSearchParams searchParams, Guid? patientId, Guid? doctorId);

        Task<int> GetTodayCountAsync();

        Task<int> GetThisMonthCountAsync(DateOnly monthStart);

        Task<IEnumerable<Appointment>> GetPatientAppointmentsForConflictCheckAsync(
            Guid patientId, 
            DateOnly date, 
            TimeSlotType timeSlot);
    }
}
