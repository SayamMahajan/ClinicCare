using ClinicCare.Shared.DTOs.Auth;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IAuthService
    {
        Task<PatientAuthResponseDto> LoginPatientAsync(LoginRequestDto dto);
        Task<EmployeeAuthResponseDto> LoginEmployeeAsync(LoginRequestDto dto);
        Task<int> RegisterPatientAsync(PatientRegisterDto dto);
        Task<int> RegisterDoctorAsync(DoctorRegisterDto dto);
        Task<int> RegisterAdminAsync(AdminRegisterDto dto);

    }
}
