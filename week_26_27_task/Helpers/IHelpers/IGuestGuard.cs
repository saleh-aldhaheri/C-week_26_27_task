using System.Security.Claims;

namespace week_26_27.Helpers.IHelpers;

public interface IGuestGuard
{
    public bool IsGuest(ClaimsPrincipal user);
}
