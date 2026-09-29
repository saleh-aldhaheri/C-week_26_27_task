using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using week_26_27.Service.IService;
using week_26_27.ViewModels;

namespace week_26_27.Areas.Identity.Controllers;

[Area(AreaConstants.IDENTITY_AREA)]
[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService, UserManager<ApplicationUser> userManager)
    {
        _profileService = profileService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {

        var user = await _userManager.GetUserAsync(User);

        if (user is null)
            return BadRequest();

        return View(model: user);
    }

    [HttpPost]
    public async Task<IActionResult> Index(ApplicationUser applicationUser)
    {
        ApplicationUser? user = null;
        try
        {
            user = await _profileService.GetProfile(applicationUser);
        }
        catch (Exception)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index), user);
    }

    [HttpPost]
    public async Task<IActionResult> ChanagePassword(ChangePasswordVM changePasswordVm)
    {
        ApplicationUser? user = await _userManager.GetUserAsync(User);

        if (user is null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            TempData[NotificationConstants.ERROR_NOTIFICATION] = string.Join(",", ModelState
                .Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList());

            return RedirectToAction(nameof(Index), user);
        }

        try
        {
            var results = await _profileService.ChangePassword(changePasswordVm, user);

            if(results is not null)
            {
                TempData[NotificationConstants.ERROR_NOTIFICATION] = string.Join(", ", results.Values.ToArray());
            }else
            {
                TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Password updated sucessfully";
            }

        }catch(Exception)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index), user);
    }
}
