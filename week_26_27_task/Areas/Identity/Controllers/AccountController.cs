using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using week_26_27.Models;
using week_26_27.ViewModels;
using week_26_27.Utilities;

namespace week_26_27.Areas.Identity.Controllers;

[Area(AreaConstants.IDENTITY_AREA)]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IRepository<ApplicationUserOtp> _applicationUserOtp;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        SignInManager<ApplicationUser> signInManager,
        IRepository<ApplicationUserOtp> applicationUserOtp
    )
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _signInManager = signInManager;
        _applicationUserOtp = applicationUserOtp;
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

        var result = await _userManager.CreateAsync(user, registerVm.Password);

        await _userManager.AddToRoleAsync(user, RoleConstants.ADMIN);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(registerVm);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var url = Url.Action(nameof(EmailConfirmation), ControllerConstants.ACCOUNT_CONTROLLER, new
        {
            area = AreaConstants.IDENTITY_AREA,
            user.Id,
            token
        }, Request.Scheme);

        string body = $"<h1>Please confirm your account by clicking <b><a href='{url}'>here</a></b></h1>";

        await _emailSender.SendEmailAsync(user.Email ?? string.Empty, "Email Confirmation", body);

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

        var user = await _userManager.FindByEmailAsync(sendEmailConfirmationVm.EmailOrUserName) ??
                   await _userManager.FindByNameAsync(sendEmailConfirmationVm.EmailOrUserName);

        if (user is null)
            return NotFound();


        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var url = Url.Action(nameof(EmailConfirmation), ControllerConstants.ACCOUNT_CONTROLLER, new
        {
            area = AreaConstants.IDENTITY_AREA,
            user.Id,
            token
        }, Request.Scheme);

        string body = $"<h1>Please confirm your account by clicking <b><a href='{url}'>here</a></b></h1>";

        await _emailSender.SendEmailAsync(user.Email ?? string.Empty, "Email Confirmation", body);

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Confirmation Mail Sent Successfully";

        return RedirectToAction(nameof(SendEmailConfirmation));
    }

    [HttpGet]
    public async Task<IActionResult> EmailConfirmation(EmailConfirmationVM emailConfirmationVm)
    {

        if (!ModelState.IsValid)
            return NotFound();

        var user = await _userManager.FindByIdAsync(emailConfirmationVm.Id);

        if (user is null)
            return NotFound();

        var result = await _userManager.ConfirmEmailAsync(user, emailConfirmationVm.Token);

        if (!result.Succeeded)
            TempData[NotificationConstants.ERROR_NOTIFICATION] = string.Join(", ", result.Errors.Select(e => e.Description));
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
            return View(nameof(Login));

        var user = await _userManager.FindByEmailAsync(loginVm.EmailOrUserName) ??
                    await _userManager.FindByNameAsync(loginVm.EmailOrUserName);

        if (user is null)
        {
            ModelState.AddModelError(nameof(loginVm.EmailOrUserName), "Incorrect Email Or UserName");

            ModelState.AddModelError(nameof(loginVm.Password), "Incorrect Password");

            return View(loginVm);
        }

        var result = await _signInManager.PasswordSignInAsync(user, loginVm.Password, loginVm.RemeberMe, true);

        if (result.IsLockedOut)
        {
            TempData[NotificationConstants.ERROR_NOTIFICATION] = "To Many Attempts Please Try Again Later";

            return View(loginVm);
        }

        if (result.IsNotAllowed)
        {
            TempData[NotificationConstants.ERROR_NOTIFICATION] = "Please Confirm Your Email";

            return View(loginVm);
        }

        if (!result.Succeeded)
        {
            ModelState.AddModelError(nameof(LoginVM.EmailOrUserName), "Invalid User Name or Email");

            ModelState.AddModelError(nameof(LoginVM.Password), "Invalid Password");

            return View(loginVm);
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = $"Welcome {user.FirstName} {user.LastName} To Your Account";

        return RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER, new
        {
            area = AreaConstants.ADMIN_AREA
        });
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

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

        var user = await _userManager.FindByEmailAsync(forgetPassword.EmailOrPassword) ??
                await _userManager.FindByNameAsync(forgetPassword.EmailOrPassword);

        if (user is null)
            return NotFound();

        string otp = new Random().Next(100000, 999999).ToString();

        await _applicationUserOtp.Add(new()
        {
            ApplicationUserId = user.Id,
        
            Otp = otp
        });

        await _applicationUserOtp.CommitAsync();

        string message = $"<h1>Your Otp is: {otp}, Please Don't Share!!!</b></h1>";

        await _emailSender.SendEmailAsync(user.Email ?? string.Empty, "Forget Password Otp", message);

        Response.Cookies.Append("userId", user.Id);

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

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();

        var dbOtp = _applicationUserOtp.Get(e =>
            e.IsUsed == false &&
            e.ExpiredAt > DateTime.UtcNow &&
            e.Otp == validateOtpVm.Otp &&
            e.ApplicationUserId == user.Id
        )
        .OrderBy(e => e.CreatedAt)
        .LastOrDefault();

        if (dbOtp is null)
        {
            ModelState.AddModelError(nameof(validateOtpVm.Otp), "Invalid Opt");
        
            return View(validateOtpVm);
        }

        dbOtp.IsUsed = true;

        await _applicationUserOtp.CommitAsync();

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

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, token, newPasswordVm.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
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
