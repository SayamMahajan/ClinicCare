using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.DTOs.Patient;
using ClinicCare.Shared.Enums;

namespace ClinicCare.Shared.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public Guid Id { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public PatientMiniDto Patient { get; set; }
        public DoctorMiniDto Doctor { get; set; }
        public PaymentType Type { get; set; }   
        public DateTime CreatedAt { get; set; }
    }
}
