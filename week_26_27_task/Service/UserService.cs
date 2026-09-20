using Microsoft.AspNetCore.Identity;
using week_26_27.Models;
using week_26_27.Service.IService;
using week_26_27.ViewModels;

namespace week_26_27.Service;

public class UserService : IUserService
{
    private UserManager<ApplicationUser> _userManager;
    private RoleManager<IdentityRole> _rolesManager;
    private readonly IPagination _pagination;

    public UserService(
        UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager, 
        IPagination pagination
    )
    {
        _userManager = userManager;
        _pagination = pagination;
        _rolesManager = roleManager;
    }

    public async Task<UserWithFilterAndPaginationVM> GetUsers(UserWithFilterAndPaginationVM userIndex)
    { 
        var query = _userManager.Users.AsQueryable();

        var searchKey = userIndex.Search?.ToLower();

        if (searchKey is not null)
            query = query.Where(e =>
                e.FirstName.ToLower().Contains(searchKey) ||
                e.LastName.ToLower().Contains(searchKey) ||
                (e.Email != null && e.Email.ToLower().Contains(searchKey)) ||
                (e.UserName != null && e.UserName.ToLower().Contains(searchKey))
            );


        int skip = (userIndex.Pagination.Page - 1) * userIndex.Pagination.PageSize;

        var users = query
            .Skip(skip)
            .Take(userIndex.Pagination.PageSize)
            .ToList();

        var userRole = new Dictionary<ApplicationUser, string>();

        foreach (var user in users)
        {
            userRole.Add(user, (await _userManager.GetRolesAsync(user)).FirstOrDefault()!);
        }

        userIndex.UsersRoles = userRole;

        userIndex.Roles = _rolesManager.Roles.ToList();

        userIndex.Pagination = _pagination.Paginate(
             count: query.Count(),
             size: userIndex.Pagination.PageSize,
             page: userIndex.Pagination.Page
        );

        return userIndex; 
    }
    
    public async Task ChangeRole(ChangeRoleVM changeRoleVm)
    {
        var user = await _userManager.FindByIdAsync(changeRoleVm.Id);

        if (user is null)
            throw new Exception();

        var role = await _rolesManager.FindByNameAsync(changeRoleVm.Role);

        if (role is null)
            throw new Exception();

        var currentRole = await _userManager.GetRolesAsync(user);

        await _userManager.RemoveFromRolesAsync(user, currentRole);

        await _userManager.AddToRoleAsync(user, changeRoleVm.Role);
    }

    public async Task<UserWithRoleVM> updateGet(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
            throw new Exception();

        var roles =  _rolesManager.Roles.ToList();

        var currentRole = (await _userManager.GetRolesAsync(user))?.FirstOrDefault();

        return new UserWithRoleVM
        {
            UserId = user.Id,
            applicationUser = user,
            SelectRoleName = currentRole ?? "",
            Roles = roles
        };
    }

    public async Task<UserWithRoleVM?> UpdateUser(UserWithRoleVM userWithRoleVm)
    {
        var role = await _rolesManager.FindByNameAsync(userWithRoleVm.SelectRoleName);
        if (role is null)
            throw new Exception();

        var user = await _userManager.FindByIdAsync(userWithRoleVm.UserId);
        if (user is null)
            throw new Exception();

        user.UserName = userWithRoleVm.applicationUser.UserName;
        user.Email = userWithRoleVm.applicationUser.Email;
        user.PhoneNumber = userWithRoleVm.applicationUser.PhoneNumber;

        IdentityResult? updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
            return userWithRoleVm;

        var currentRoles = await _userManager.GetRolesAsync(user);

        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        await _userManager.AddToRoleAsync(user, userWithRoleVm.SelectRoleName);

        return null;
    }
    
    public async Task Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
            throw new Exception();

        await _userManager.DeleteAsync(user);
    }
}
