using week_26_27.ViewModels;

namespace week_26_27.Service.IService;

public interface IAccountService
{
	public Task<Dictionary<string, string>?> Register(RegisterVM registerVm, Func<string, string, string> contructUrl);
	public Task<Dictionary<string, string>?> SendEmailConfirmation(SendEmailConfirmationVM sendEmailConfirmationVm, Func<string, string, string> contructUrl);
	public Task<Dictionary<string, string>?> EmailConfirmation(EmailConfirmationVM emailConfirmationVm);
	public Task<Dictionary<string, string>?> Login(LoginVM loginVm);
	public Task Logout();
	public Task<string> ForgetPassword(ForgetPasswordVM forgetPasswordVm);
	public Task<Dictionary<string, string>?> ValidateOtp(ValidateOtpVM validateOtpVm, string userId);
	public Task<Dictionary<string, string>?> NewPassword(NewPasswordVM newPasswordVm, string userId);
}
