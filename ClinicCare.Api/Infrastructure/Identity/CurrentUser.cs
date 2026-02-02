using System.Security.Claims;
using ClinicCare.Business.Interfaces;
using ClinicCare.Shared.Enums;

namespace ClinicCare.Api.Infrastructure.Security;

public class CurrentUser : ICurrentUser
{
    public Guid UserId { get; }
    public UserRole Role { get; }
    public string Email { get; } = string.Empty;
    public bool IsAuthenticated { get; }

    public CurrentUser(IHttpContextAccessor accessor)
    {
        var user = accessor.HttpContext?.User;

        IsAuthenticated = user?.Identity?.IsAuthenticated ?? false;

        if (!IsAuthenticated)
            return;

        var userIdClaim = user!.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdClaim, out var id))
            UserId = id;

        var roleClaim = user!.FindFirstValue(ClaimTypes.Role);
        if (Enum.TryParse<UserRole>(roleClaim, true, out var role))
            Role = role;

        Email = user!.FindFirstValue(ClaimTypes.Email)!;
    }
}
