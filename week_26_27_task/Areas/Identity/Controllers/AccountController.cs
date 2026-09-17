using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using week_26_27.Models;
using week_26_27.ViewModels;

namespace week_26_27.Areas.Identity.Controllers;

[Area(AreaConstants.IDENTITY_AREA)]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM registerVm)
    {
        if (!ModelState.IsValid)
            return View(registerVm);

        var user = registerVm.Adapt<ApplicationUser>();

        var result = await _userManager.CreateAsync(user,registerVm.Password);

        if(!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(registerVm);
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Account Created successfully";

        return RedirectToAction(nameof(Login));
    }

    public IActionResult Login()
    {
        return View();
    }
}
