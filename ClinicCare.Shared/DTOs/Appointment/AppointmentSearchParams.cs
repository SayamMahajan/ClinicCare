using ClinicCare.Shared.DTOs.Pagination;
namespace ClinicCare.Shared.Enums
{
    public class AppointmentSearchParams : PaginationParams
    {
        public string? SearchTerm { get; set; } 
        public AppointmentStatus? Status { get; set; }
        public Guid? PaymentId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
