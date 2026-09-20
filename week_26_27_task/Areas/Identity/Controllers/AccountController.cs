using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using week_26_27.Models;
using week_26_27.ViewModels;
using week_26_27.Utilities;
using week_26_27.Service.IService;

namespace week_26_27.Areas.Identity.Controllers;

[Area(AreaConstants.IDENTITY_AREA)]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAccountService _accountService;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        IAccountService accountSerivce
    )
    {
        _userManager = userManager;
        _accountService = accountSerivce; 
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

       var result = await _accountService.Register(registerVm, (token, userId) => Url.Action(
        nameof(EmailConfirmation),
        ControllerConstants.ACCOUNT_CONTROLLER,
        new
        {
            area = AreaConstants.IDENTITY_AREA,
            userId,
            token
        },
        Request.Scheme
        )!);

        if(result is not null)
        {
            foreach(var error in result)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }

            return View(registerVm);
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Account Created successfully";

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult SendEmailConfirmation()
    {

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SendEmailConfirmation(SendEmailConfirmationVM sendEmailConfirmationVm)
    {
        if (!ModelState.IsValid)
            return View(sendEmailConfirmationVm);

        try
        {
            await _accountService.SendEmailConfirmation(sendEmailConfirmationVm, (token, userId) => Url.Action(
                nameof(EmailConfirmation),
                ControllerConstants.ACCOUNT_CONTROLLER,
                new
                {
                    area = AreaConstants.IDENTITY_AREA,
                    userId,
                    token
                },
                Request.Scheme
            )!);
        }
        catch (Exception)
        {
            return NotFound();
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Confirmation Mail Sent Successfully";

        return RedirectToAction(nameof(SendEmailConfirmation));
    }

    [HttpGet]
    public async Task<IActionResult> EmailConfirmation(EmailConfirmationVM emailConfirmationVm)
    {

        if (!ModelState.IsValid)
            return NotFound();

        Dictionary<string, string>? result;

        try
        {
            result = await _accountService.EmailConfirmation(emailConfirmationVm);
        }
        catch (Exception)
        {
            return NotFound();
        }

        if (result is not null)
            TempData[NotificationConstants.ERROR_NOTIFICATION] = string.Join(", ", result.Values);
        else
            TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Email Confrimed";

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM loginVm)
    {
        if (!ModelState.IsValid)
            return View(loginVm);

        var result = await _accountService.Login(loginVm);

        if (result is not null)
        {
            foreach (var error in result)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }

            return View(loginVm);
        }

        var user = await _userManager.FindByEmailAsync(loginVm.EmailOrUserName) ??
                   await _userManager.FindByNameAsync(loginVm.EmailOrUserName);

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = $"Welcome {user!.FirstName} {user.LastName} To Your Account";

        return RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER, new
        {
            area = AreaConstants.ADMIN_AREA
        });
    }

    public async Task<IActionResult> Logout()
    {
        await _accountService.Logout();

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Logut Succesfully";

        return View(nameof(Login));
    }

    [HttpGet]
    public IActionResult ForgetPassword()
    {

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPassword)
    {
        if (!ModelState.IsValid)
            return View(forgetPassword);

        string userId;

        try
        {
            userId = await _accountService.ForgetPassword(forgetPassword);
        }
        catch (Exception)
        {
            return NotFound();
        }

        Response.Cookies.Append("userId", userId);

        TempData["otp_validation"] = Guid.NewGuid().ToString();

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Otp Sent Succsfully";

        return RedirectToAction(nameof(ValidateOtp));
    }

    [HttpGet]
    public IActionResult ValidateOtp()
    {

        if (TempData["otp_validation"] is null)
            return NotFound();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ValidateOtp(ValidateOtpVM validateOtpVm)
    {
        if (!ModelState.IsValid)
            return View(validateOtpVm);

        var userId = Request.Cookies["userId"];

        if (userId is null)
            return NotFound();

        Dictionary<string, string>? result;

        try
        {
            result = await _accountService.ValidateOtp(validateOtpVm, userId);
        }
        catch (Exception)
        {
            return NotFound();
        }

        if (result is not null)
        {
            foreach (var error in result)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }

            return View(validateOtpVm);
        }

        return RedirectToAction(nameof(NewPassword));
    }

    [HttpGet]
    public IActionResult NewPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> NewPassword(NewPasswordVM newPasswordVm)
    {
        if (!ModelState.IsValid)
            return View(newPasswordVm);

        var userId = Request.Cookies["userId"];

        if (userId is null)
            return NotFound();

        Dictionary<string, string>? result;

        try
        {
            result = await _accountService.NewPassword(newPasswordVm, userId);
        }
        catch (Exception)
        {
            return NotFound();
        }

        if (result is not null)
        {
            foreach (var error in result)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }

            return View(newPasswordVm);
        }

        Response.Cookies.Delete("userId");

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Password Changed Succsfully";

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
