using Microsoft.AspNetCore.Identity;
using week_26_27.Models;

namespace week_26_27.ViewModels;

public class UserWithFilterAndPaginationVM
{
    public Dictionary<ApplicationUser, string> UsersRoles { get; set; } = new Dictionary<ApplicationUser, string>();
    public string Search { get; set; } = string.Empty;
    public ICollection<IdentityRole> Roles = new List<IdentityRole>();
    public PaginationVM Pagination { get; set; } = new PaginationVM();
}
