using ClinicCare.Shared.DTOs.Pagination;

namespace ClinicCare.Shared.DTOs.Prescription
{
    public class PrescriptionSearchParams : PaginationParams
    {
        public string? SearchTerm { get; set; }   
        public Guid? AppointmentId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }

}
