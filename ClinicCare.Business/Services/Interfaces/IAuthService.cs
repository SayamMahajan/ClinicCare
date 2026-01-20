using ClinicCare.Shared.DTOs.Auth;

namespace ClinicCare.Business.Services.Interfaces
{
    public interface IAuthService
    {
        Task<PatientAuthResponseDto> LoginPatientAsync(LoginRequestDto dto);
        Task<EmployeeAuthResponseDto> LoginEmployeeAsync(LoginRequestDto dto);
        Task<Guid> RegisterPatientAsync(PatientRegisterDto dto);
        Task<Guid> RegisterDoctorAsync(DoctorRegisterDto dto);
        Task<Guid> RegisterAdminAsync(AdminRegisterDto dto);

    }
}
