using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using week_26_27.Models;
using week_26_27.Service.IService;
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

    private readonly IUserService _userService;

    public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IPagination pagination, IUserService userService)
    {
        _userManager = userManager;
        _pagination = pagination;
        _rolesManager = roleManager;
        _userService = userService;
    }

    public async Task<IActionResult> Index(UserWithFilterAndPaginationVM userIndex)
    {
        userIndex = await _userService.GetUsers(userIndex);

        return View(model: userIndex);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleConstants.SUPER_ADMIN)]
    public async Task<IActionResult> ChangeRole([FromForm] ChangeRoleVM changeRoleVm)
    {
        try
        {
            await _userService.ChangeRole(changeRoleVm);

        }
        catch (Exception)
        {
            NotFound();
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Role Changed Sucssfully";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = RoleConstants.SUPER_ADMIN)]
    public async Task<IActionResult> Update(string id)
    {
        try
        {
            var userWithRoleVm = await _userService.updateGet(id);
            return View(model: userWithRoleVm);
        }
        catch(Exception)
        {
            return NotFound();
        }
      
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.SUPER_ADMIN)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(UserWithRoleVM userWithRoleVm)
    {
        try
        {
            var result = await _userService.UpdateUser(userWithRoleVm);

            if (result is not null)
                return View(result);

        }
        catch (Exception)
        {
            return NotFound();
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "User Updated Successfully";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = RoleConstants.SUPER_ADMIN)]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _userService.Delete(id);

        }catch(Exception)
        {
            return NotFound();
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "User Delete Succsfully";

        return RedirectToAction(nameof(Index));
    }
}