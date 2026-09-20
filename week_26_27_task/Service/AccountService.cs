using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using week_26_27.Models;
using week_26_27.Service.IService;
using week_26_27.Utilities;
using week_26_27.ViewModels;

namespace week_26_27.Service;

public class AccountSerivce : IAccountService
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IEmailSender _emailSender;
	private readonly SignInManager<ApplicationUser> _signInManager;
	private readonly IRepository<ApplicationUserOtp> _applicationUserOtp;

	public AccountSerivce(
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

	public async Task<Dictionary<string, string>?> Register(RegisterVM registerVm, Func<string, string, string> contructUrl)
	{
		var user = registerVm.Adapt<ApplicationUser>();

		var result = await _userManager.CreateAsync(user, registerVm.Password);

		if (!result.Succeeded)
			return ToErrorDictionary(result);

		await _userManager.AddToRoleAsync(user, RoleConstants.ADMIN);

		var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

		var url = contructUrl(token, user.Id);

		string body = $"<h1>Please confirm your account by clicking <b><a href='{url}'>here</a></b></h1>";

		await _emailSender.SendEmailAsync(user.Email ?? string.Empty, "Email Confirmation", body);

		return null;
	}

	public async Task<Dictionary<string, string>?> SendEmailConfirmation(SendEmailConfirmationVM sendEmailConfirmationVm, Func<string, string, string> contructUrl)
	{
		var user = await _userManager.FindByEmailAsync(sendEmailConfirmationVm.EmailOrUserName) ??
				   await _userManager.FindByNameAsync(sendEmailConfirmationVm.EmailOrUserName);

		if (user is null)
			throw new Exception();

		var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

		var url = contructUrl(token, user.Id);

		string body = $"<h1>Please confirm your account by clicking <b><a href='{url}'>here</a></b></h1>";

		await _emailSender.SendEmailAsync(user.Email ?? string.Empty, "Email Confirmation", body);

		return null;
	}

	public async Task<Dictionary<string, string>?> EmailConfirmation(EmailConfirmationVM emailConfirmationVm)
	{
		var user = await _userManager.FindByIdAsync(emailConfirmationVm.Id);

		if (user is null)
			throw new Exception();

		var result = await _userManager.ConfirmEmailAsync(user, emailConfirmationVm.Token);

		if (!result.Succeeded)
			return ToErrorDictionary(result);

		return null;
	}

	public async Task<Dictionary<string, string>?> Login(LoginVM loginVm)
	{
		var user = await _userManager.FindByEmailAsync(loginVm.EmailOrUserName) ??
				   await _userManager.FindByNameAsync(loginVm.EmailOrUserName);

		if (user is null)
			return new Dictionary<string, string>
			{
				{ nameof(loginVm.EmailOrUserName), "Incorrect Email Or UserName" },
				{ nameof(loginVm.Password), "Incorrect Password" }
			};

		var result = await _signInManager.PasswordSignInAsync(user, loginVm.Password, loginVm.RemeberMe, true);

		if (result.IsLockedOut)
			return new Dictionary<string, string>
			{
				{ string.Empty, "To Many Attempts Please Try Again Later" }
			};

		if (result.IsNotAllowed)
			return new Dictionary<string, string>
			{
				{ string.Empty, "Please Confirm Your Email" }
			};

		if (!result.Succeeded)
			return new Dictionary<string, string>
			{
				{ nameof(LoginVM.EmailOrUserName), "Invalid User Name or Email" },
				{ nameof(LoginVM.Password), "Invalid Password" }
			};

		return null;
	}

	public async Task Logout()
	{
		await _signInManager.SignOutAsync();
	}

	public async Task<string> ForgetPassword(ForgetPasswordVM forgetPasswordVm)
	{
		var user = await _userManager.FindByEmailAsync(forgetPasswordVm.EmailOrPassword) ??
				   await _userManager.FindByNameAsync(forgetPasswordVm.EmailOrPassword);

		if (user is null)
			throw new Exception();

		string otp = new Random().Next(100000, 999999).ToString();

		await _applicationUserOtp.Add(new()
		{
			ApplicationUserId = user.Id,
			Otp = otp
		});

		await _applicationUserOtp.CommitAsync();

		string message = $"<h1>Your Otp is: {otp}, Please Don't Share!!!</b></h1>";

		await _emailSender.SendEmailAsync(user.Email ?? string.Empty, "Forget Password Otp", message);

		return user.Id;
	}

	public async Task<Dictionary<string, string>?> ValidateOtp(ValidateOtpVM validateOtpVm, string userId)
	{
		var user = await _userManager.FindByIdAsync(userId);

		if (user is null)
			throw new Exception();

		var dbOtp = _applicationUserOtp.Get(e =>
			e.IsUsed == false &&
			e.ExpiredAt > DateTime.UtcNow &&
			e.Otp == validateOtpVm.Otp &&
			e.ApplicationUserId == user.Id
		)
		.OrderBy(e => e.CreatedAt)
		.LastOrDefault();

		if (dbOtp is null)
			return new Dictionary<string, string>
			{
				{ nameof(validateOtpVm.Otp), "Invalid Opt" }
			};

		dbOtp.IsUsed = true;

		await _applicationUserOtp.CommitAsync();

		return null;
	}

	public async Task<Dictionary<string, string>?> NewPassword(NewPasswordVM newPasswordVm, string userId)
	{
		var user = await _userManager.FindByIdAsync(userId);

		if (user is null)
			throw new Exception();

		var token = await _userManager.GeneratePasswordResetTokenAsync(user);

		var result = await _userManager.ResetPasswordAsync(user, token, newPasswordVm.Password);

		if (!result.Succeeded)
			return ToErrorDictionary(result);

		return null;
	}

	private static Dictionary<string, string> ToErrorDictionary(IdentityResult result)
	{
		var errorDict = new Dictionary<string, string>();

		foreach (var error in result.Errors)
			errorDict.TryAdd(string.Empty, error.Description);

		return errorDict;
	}
}
