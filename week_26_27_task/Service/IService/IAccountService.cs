
using Microsoft.AspNetCore.Identity;
using week_26_27.ViewModels;

namespace week_26_27.Service.IService;

public interface IAccountService<TUser> where TUser : class
{
    public Task<(IdentityResult?, TUser)> Register(RegisterVM registerVm);

    public Task<string> GenerateEmailConfirmationTokenAsync(TUser user);

    public Task<(IdentityResult?, TUser)> ConfirmEmail(string id, string token);
    
    public Task<(TUser, string)> ResendEmailConfirmation(string emailOrUsername);

    public Task<Dictionary<string, string>?> Login(LoginVM loginVM);

    public Task Logout();
}
