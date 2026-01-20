using System.Security.Claims;
using ClinicCare.Business.Interfaces;
using ClinicCare.Shared.Enums;

namespace ClinicCare.Api.Infrastructure.Security;

public class CurrentUser : ICurrentUser
{
    public Guid UserId { get; }
    public UserRole Role { get; }
    public string Email { get; }

    public CurrentUser(IHttpContextAccessor accessor)
    {
        var user = accessor.HttpContext?.User
                ?? throw new UnauthorizedAccessException();

        UserId = Guid.Parse(
            user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        Role = Enum.Parse<UserRole>(
            user.FindFirstValue(ClaimTypes.Role)!,
            ignoreCase: true
        );

        Email = user.FindFirstValue(ClaimTypes.Email)!;
    }
}
