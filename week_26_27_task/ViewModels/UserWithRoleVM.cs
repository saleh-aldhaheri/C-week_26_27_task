using Microsoft.AspNetCore.Identity;
using week_26_27.Models;

namespace week_26_27.ViewModels;

public class UserWithRoleVM
{
    public string UserId { get; set;  } = string.Empty;
    public ApplicationUser applicationUser { get; set; } = null!;
    public string SelectRoleName { get; set; } = string.Empty;
    public ICollection<IdentityRole> Roles { get; set; } = new List<IdentityRole>();
}
