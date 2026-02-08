using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.Enums;

namespace ClinicCare.Shared.DTOs.Payment
{
    public class PaymentSearchParams : PaginationParams
    {
        public string? SearchTerm { get; set; }
        public Guid? PatientId { get; set; }  
        public Guid? DoctorId { get; set; }
        public PaymentType? Type { get; set; }   
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }

}
