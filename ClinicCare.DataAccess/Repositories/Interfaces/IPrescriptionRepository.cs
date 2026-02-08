using ClinicCare.DataAccess.Models;
using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.DTOs.Prescription;

namespace ClinicCare.DataAccess.Repositories.Interfaces
{
    public interface IPrescriptionRepository : IGenericRepository<Prescription>
    {
        Task<PaginatedResult<Prescription>> GetAllAsync(PrescriptionSearchParams searchParams, Guid? patientId = null, Guid? doctorId = null);

    }
}
