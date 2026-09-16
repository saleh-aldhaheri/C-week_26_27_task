using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using week_26_27.Models;
using week_26_27.Service.IService;
using week_26_27.ViewModels;

namespace week_26_27.Areas.Identity.Controllers;

[Area(AreaConstants.IDENTITY_AREA)]
public class AccountController : Controller
{
    public IAccountService<ApplicationUser> _accountService;
    private IEmailSender _emailSenderService;
    private UserManager<ApplicationUser> _userManager;
    private IRepository<ApplicationUserOtp> _applicationUserOtp;

    public AccountController(
        IAccountService<ApplicationUser> accountService,
        IEmailSender emailSenderService,
        UserManager<ApplicationUser> userManager,
        IRepository<ApplicationUserOtp> applicationUserOtp
    )
    {
        _accountService = accountService;
        _emailSenderService = emailSenderService;
        _userManager = userManager;
        _applicationUserOtp = applicationUserOtp;
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity is not null && User.Identity.IsAuthenticated)
            return RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER, new
            {
                area = AreaConstants.ADMIN_AREA
            });

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM registerVm)
    {
        if (User.Identity is not null && User.Identity.IsAuthenticated)
            return RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER, new
            {
                area = AreaConstants.ADMIN_AREA
            });

        if (!ModelState.IsValid)
            return View(registerVm);

        IdentityResult? result = null;
        ApplicationUser? user = null; 

        try
        {
            (result, user) = await _accountService.Register(registerVm);
        }
        catch (Exception)
        {
            ModelState.AddModelError(nameof(registerVm.Email), "Invalid Email");
            return View(registerVm);
        }

       
        if(result is not null && !result.Succeeded)
        {
            foreach(var item in result.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
            }

            return View(registerVm); 
        }

        var token = await _accountService.GenerateEmailConfirmationTokenAsync(user);
        var link = Url.Action(nameof(EmailConfirmation), ControllerConstants.ACCONT_CONTROLLER, new { area = AreaConstants.IDENTITY_AREA, id = (user as ApplicationUser)?.Id, token }, Request.Scheme);
        var message = $"<h1>Please confirm your account by clicking <b><a href='{link}'>here</a></b></h1>";
        await _emailSenderService.SendEmailAsync((user as ApplicationUser)?.Email ?? string.Empty, "Confirm your account", message);


        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Account Create Please Validate Your Email";

        return RedirectToAction(nameof(Login));
    }

    public async Task<IActionResult> EmailConfirmation(string id, string token)
    {
        if (User.Identity is not null && User.Identity.IsAuthenticated)
            return  RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER, new
            {
                area = AreaConstants.ADMIN_AREA
            });

        try
        {
            var (result, user) = await _accountService.ConfirmEmail(id, token);

            if (result is not null && !result.Succeeded)
                TempData[NotificationConstants.ERROR_NOTIFICATION] = String.Join(", ", result.Errors.Select(e => e.Description));
            else
            {
                TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Confirm Account successfully, please login";
            }
        }
        catch (Exception)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult ResendEmailConfirmation()
    {
        if (User.Identity is not null && User.Identity.IsAuthenticated)
           return RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER, new
            {
                area = AreaConstants.ADMIN_AREA
            });

        return View(); 
    }

    [HttpPost]
    public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM emailConfirmationVm)
    {
        if (User.Identity is not null && User.Identity.IsAuthenticated)
            return RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER, new
            {
                area = AreaConstants.ADMIN_AREA
            });

        if (!ModelState.IsValid)
            return View(emailConfirmationVm);
        
        ApplicationUser? user = null;
        string token = "";

        try
        {
            (user, token) = await _accountService.ResendEmailConfirmation(emailConfirmationVm.EmailOrUsername);
        }
        catch (Exception)
        {
            return NotFound(); 
        }

        
        var link = Url.Action(nameof(EmailConfirmation), ControllerConstants.ACCONT_CONTROLLER, new {
            area = AreaConstants.IDENTITY_AREA, 
            id = (user as ApplicationUser)?.Id, 
            token }, Request.Scheme);

        var message = $"<h1>Please confirm your account by clicking <b><a href='{link}'>here</a></b></h1>";

        await _emailSenderService.SendEmailAsync(user.Email ?? string.Empty, "Confirm your account", message);

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Email Confirmation Sent Sucessfully"; 

        return RedirectToAction(nameof(Login));  
    }


    [HttpGet]
    public IActionResult Login()
    {

        if (User.Identity is not null && User.Identity.IsAuthenticated)
            return RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER, new
            {
                area = AreaConstants.ADMIN_AREA
            });

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM loginVm)
    {
        if (User.Identity is not null && User.Identity.IsAuthenticated)
            return RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER, new
            {
                area = AreaConstants.ADMIN_AREA
            });

        if (!ModelState.IsValid)
            return View(loginVm);

        try
        {
            var errors = await _accountService.Login(loginVm);

            if (errors is not null)
            {
                foreach (var err in errors)
                    ModelState.AddModelError(err.Key, err.Value);

                return View(loginVm);
            }
        }
        catch(Exception)
        {
            return NotFound();
        }
        

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Login Successful";

       return RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER,new
       {
           area = AreaConstants.ADMIN_AREA
       });
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        if (User.Identity is null || !User.Identity.IsAuthenticated)
           return RedirectToAction(nameof(Index), ControllerConstants.ACCONT_CONTROLLER, new
            {
                area = AreaConstants.IDENTITY_AREA
            });

        await _accountService.Logout();

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "User Logout Sucessfully";

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult ForgetPassword()
    {
        if (User.Identity is not null && User.Identity.IsAuthenticated)
            return RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER, new
            {
                area = AreaConstants.ADMIN_AREA
            });

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPassword)
    {
        if (User.Identity is not null && User.Identity.IsAuthenticated)
            return RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER, new
            {
                area = AreaConstants.ADMIN_AREA
            });

        if (!ModelState.IsValid)
            return View(forgetPassword);

        var user = await _userManager.FindByEmailAsync( forgetPassword.EmailOrUsername) ?? 
                   await _userManager.FindByNameAsync( forgetPassword.EmailOrUsername );

        if (user is null)
            return NotFound();

        var otp = new Random().Next(100000,999999).ToString();

        await _applicationUserOtp.Add(new ApplicationUserOtp
        {
            ApplicationUserId = user.Id,
            Otp = otp
        });

        await _applicationUserOtp.CommitAsync();

        string message = $"<h1> Your Otp is: {otp} \nPlease Don't Share It With Anyone!";

        TempData["validate_otp"] = Guid.NewGuid().ToString();

        Response.Cookies.Append("userId", user.Id);

        await _emailSenderService.SendEmailAsync(user.Email, "Reset Password", message);

        return RedirectToAction(nameof(ValidateOtp));
    }

    [HttpGet]
    public IActionResult ValidateOtp()
    {
        if (User.Identity is not null && User.Identity.IsAuthenticated)
            return RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER, new
            {
                area = AreaConstants.ADMIN_AREA
            });

        if (TempData["validate_otp"] is null)
            return NotFound();

        return View(); 
    }

    [HttpPost]
    public async Task<IActionResult> ValidateOtp(ValidateOtpVM validateOtp)
    {
        if (User.Identity is not null && User.Identity.IsAuthenticated)
            return RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER, new
            {
                area = AreaConstants.ADMIN_AREA
            });

        if (!ModelState.IsValid)
            return View(validateOtp);

        var userId = Request.Cookies["UserId"];

        if (userId is null)
            return NotFound();

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();

        var DbOtp = await _applicationUserOtp.Get(e => 
            e.IsUsed == false &&
            e.ApplicationUserId == user.Id &&
            e.ExpiredAt > DateTime.UtcNow &&
            e.Otp == validateOtp.Otp
        )
        .OrderBy(e => e.CreateAt)
        .LastOrDefaultAsync();

        if(DbOtp is null)
        {
            TempData[NotificationConstants.ERROR_NOTIFICATION] = "Invalid Otp Number";
            TempData["validate_otp"] = Guid.NewGuid().ToString();

            return RedirectToAction(nameof(ValidateOtp));
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = $"Valid OTP, you can now change your password";
        DbOtp.IsUsed = true;
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
        
        var userId = Request.Cookies["userId"];

        if (userId is null)
            return NotFound();

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user,token, newPasswordVm.NewPassword);

        Response.Cookies.Delete("userId");

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Password Chanaged Succsfully";

        return View(nameof(Login));
    }
}