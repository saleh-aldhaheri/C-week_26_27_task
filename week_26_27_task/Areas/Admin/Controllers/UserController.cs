using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using week_26_27.Models;
using week_26_27.Utilities;
using week_26_27.ViewModels;

namespace week_26_27.Areas.Admin.Controllers;

[Area(AreaConstants.ADMIN_AREA)]
[Authorize(Roles = $"{RoleConstants.ADMIN},{RoleConstants.SUPER_ADMIN}")]
public class UserController : Controller
{
    private UserManager<ApplicationUser> _userManager;
    private RoleManager<IdentityRole> _rolesManager; 
    private readonly IPagination _pagination;

    public UserController(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager, IPagination pagination)
    {
        _userManager = userManager;
        _pagination = pagination;
        _rolesManager = roleManager;
    }
    
    public async Task<IActionResult> Index(UserWithFilterAndPaginationVM userIndex)
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

        var users = await query.Skip(skip).Take(userIndex.Pagination.PageSize).ToListAsync();

        var userRole = new Dictionary<ApplicationUser, string>();

        foreach(var user in users)
        {
            userRole.Add(user, (await _userManager.GetRolesAsync(user)).FirstOrDefault()!);
        }

        userIndex.UsersRoles = userRole;

        userIndex.Roles = _rolesManager.Roles.ToList();

        userIndex.Pagination = _pagination.Paginate(
             count: query.Count(),
             size:  userIndex.Pagination.PageSize,
             page: userIndex.Pagination.Page
        );

        return View(model: userIndex);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleConstants.SUPER_ADMIN)]
    public async Task<IActionResult> ChangeRole([FromForm] ChangeRoleVM changeRoleVm)
    {
        var user = await _userManager.FindByIdAsync(changeRoleVm.Id);

        if (user is null)
            return NotFound();

        var role = await _rolesManager.FindByNameAsync(changeRoleVm.Role);

        if (role is null)
            return NotFound();

        var currentRole = await _userManager.GetRolesAsync(user);

        await _userManager.RemoveFromRolesAsync(user, currentRole);

        await _userManager.AddToRoleAsync(user, changeRoleVm.Role);

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Role Changed Sucssfully";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = RoleConstants.SUPER_ADMIN)]
    public async Task<IActionResult> Update(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
            return NotFound();
        var roles = await _rolesManager.Roles.ToListAsync();

        var currentRole = (await _userManager.GetRolesAsync(user))?.FirstOrDefault();

        return View(model: new UserWithRoleVM
        {
            UserId = user.Id,
            applicationUser = user,
            SelectRoleName = currentRole ?? "",
            Roles = roles
        }); 
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.SUPER_ADMIN)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(UserWithRoleVM userWithRoleVm)
    {
        if (!ModelState.IsValid)
            return View(userWithRoleVm);

        var role = await _rolesManager.FindByNameAsync(userWithRoleVm.SelectRoleName);
        if (role is null)
            return NotFound();

        var user = await _userManager.FindByIdAsync(userWithRoleVm.UserId);
        if (user is null)
            return NotFound();

        user.UserName = userWithRoleVm.applicationUser.UserName;
        user.Email = userWithRoleVm.applicationUser.Email;
        user.PhoneNumber = userWithRoleVm.applicationUser.PhoneNumber;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
            return View(userWithRoleVm);

        var currentRoles = await _userManager.GetRolesAsync(user);

        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        
        await _userManager.AddToRoleAsync(user, userWithRoleVm.SelectRoleName);

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "User Updated Successfully";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = RoleConstants.SUPER_ADMIN)]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
            return NotFound();

        await _userManager.DeleteAsync(user);

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "User Delete Succsfully";

        return RedirectToAction(nameof(Index));
    }
}