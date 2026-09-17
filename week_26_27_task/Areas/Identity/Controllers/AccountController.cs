using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using week_26_27.Models;
using week_26_27.ViewModels;

namespace week_26_27.Areas.Identity.Controllers;

[Area(AreaConstants.IDENTITY_AREA)]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender; 

    public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _emailSender = emailSender;
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
}
