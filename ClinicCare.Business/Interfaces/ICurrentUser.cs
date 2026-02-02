using ClinicCare.Shared.Enums;

namespace ClinicCare.Business.Interfaces
{
    public interface ICurrentUser
    {
        Guid UserId { get; }
        UserRole Role { get; }
        string Email { get; }
        bool IsAuthenticated { get; }
    }
}
