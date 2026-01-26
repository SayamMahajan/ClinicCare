using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.DTOs.Patient;

namespace ClinicCare.Shared.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public PatientMiniDto Patient { get; set; }
        public DoctorMiniDto Doctor { get; set; }
    }
}
