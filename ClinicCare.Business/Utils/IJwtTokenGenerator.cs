using ClinicCare.DataAccess.Models;

namespace ClinicCare.Business.Utils
{
    public interface IJwtTokenGenerator
    {
        string GeneratePatientToken(Patient patient);
        string GenerateEmployeeToken(Employee employee);
    }
}