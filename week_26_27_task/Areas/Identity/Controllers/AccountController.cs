using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using week_26_27.Models;
using week_26_27.ViewModels;

namespace week_26_27.Areas.Identity.Controllers;

[Area(AreaConstants.IDENTITY_AREA)]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender; 

    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        SignInManager<ApplicationUser> signInManager
    )
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _signInManager = signInManager;
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

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var url = Url.Action(nameof(EmailConfirmation), ControllerConstants.ACCOUNT_CONTROLLER, new
        {
            area = AreaConstants.IDENTITY_AREA,
            user.Id,
            token
        }, Request.Scheme);

        string body = $"<h1>Please confirm your account by clicking <b><a href='{url}'>here</a></b></h1>";

        await _emailSender.SendEmailAsync(user.Email ?? string.Empty,"Email Confirmation", body);

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

    public async Task<IActionResult> Login(LoginVM loginVm)
    {
        if (!ModelState.IsValid)
            return View(nameof(Login));

        var user = await _userManager.FindByEmailAsync(loginVm.EmailOrUserName) ??
                    await _userManager.FindByNameAsync(loginVm.EmailOrUserName);

        if(user is null)
        {
            ModelState.AddModelError(nameof(loginVm.EmailOrUserName), "Incorrect Email Or UserName");
            ModelState.AddModelError(nameof(loginVm.Password), "Incorrect Password");
            return View(loginVm);
        }

        var result = await _signInManager.PasswordSignInAsync(user, loginVm.Password, loginVm.RemeberMe, true);

        if(result.IsLockedOut)
        {
            TempData[NotificationConstants.ERROR_NOTIFICATION] = "To Many Attempts Please Try Again Later";
            return View(loginVm);
        }

        if(result.IsNotAllowed)
        {
            TempData[NotificationConstants.ERROR_NOTIFICATION] = "Please Confirm Your Email";
            return View(loginVm);
        }

        if(!result.Succeeded)
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
}
