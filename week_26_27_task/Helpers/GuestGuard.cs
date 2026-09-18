using System.Security.Claims;
using week_26_27.Helpers.IHelpers;

namespace week_26_27.Helpers;

public class GuestGuard : IGuestGuard
{
    public bool IsGuest(ClaimsPrincipal user)
    {
        return user.Identity is null || !user.Identity.IsAuthenticated;
    }
}
