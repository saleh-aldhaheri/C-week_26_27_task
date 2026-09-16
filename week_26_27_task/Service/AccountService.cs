using Mapster;
using Microsoft.AspNetCore.Identity;
using week_26_27.Service.IService;
using week_26_27.ViewModels;

namespace week_26_27.Service;


public class AccountService<TUser> : IAccountService<TUser> where TUser : class
{
    private UserManager<TUser> _userManager;
    private SignInManager<TUser> _signInManager;
    
    public AccountService(UserManager<TUser> userManager, SignInManager<TUser> signInManager)
    {
        _userManager = userManager; 
        _signInManager = signInManager;
    }

    public async Task<(IdentityResult?, TUser)> Register(RegisterVM registerVm)
    {
        var user = registerVm.Adapt<TUser>();

        var exstingUser = await _userManager.FindByEmailAsync(registerVm.Email) ??
                           await _userManager.FindByNameAsync(registerVm.UserName);

        if (exstingUser is not null)
            throw new Exception();

        var result = await _userManager.CreateAsync(user, registerVm.Password);

        return (result, user);
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(TUser user)
    {
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<(IdentityResult?, TUser)> ConfirmEmail(string id, string token)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
            throw new Exception("user not exist");

        var result = await _userManager.ConfirmEmailAsync(user, token);

        return (result, user);
    }

    public async Task<(TUser, string)> ResendEmailConfirmation(string emailOrUseranme)
    {
        var user = await _userManager.FindByEmailAsync(emailOrUseranme) ?? 
                   await _userManager.FindByNameAsync(emailOrUseranme);

        if (user is null)
            throw new Exception();

        var token = await GenerateEmailConfirmationTokenAsync(user);

        return (user, token);
    }

    /// <summary>
    /// Authenticates a user based on their login credentials.
    /// </summary>
    /// <param name="loginVm">The login view model containing the user's credentials.</param>
    /// <returns>
    /// A <see cref="Dictionary{Tkey,TValue}"/> containing validation errors if authentication fails; 
    /// otherwise, <see langword="null"/> if the login is successful.
    /// </returns>
    /// <exception cref="Exception">Throws When The User Not Found.</exception>
    public async Task<Dictionary<string, string>?> Login(LoginVM loginVm)
    {
        var user = await _userManager.FindByEmailAsync(loginVm.EmailOrUsername) ??
        await _userManager.FindByNameAsync(loginVm.EmailOrUsername);

        Dictionary<string, string> errors = new Dictionary<string, string>();
        
        if (user is null)
            throw new Exception();

        var signInResult = await _signInManager.PasswordSignInAsync(user, loginVm.Password, loginVm.RememberMe, true);

        if (signInResult.IsLockedOut)
        {
            errors[""] = "Exceded the attempt limit please Try Later";
            return errors;
        }

        if (!signInResult.Succeeded)
        {
            errors[nameof(loginVm.EmailOrUsername)] = "Wrong Email or Username";
            errors[nameof(loginVm.Password)] = "Wrong Pasword";
            return errors;
        }

        if (signInResult.IsNotAllowed)
        {
            errors[nameof(loginVm.EmailOrUsername)] = "Please Validate Your Email First";
            errors[nameof(loginVm.Password)] = "Wrong Pasword";
            return errors;
        }

        return null;

    }

    public async Task Logout()
    {
       await _signInManager.SignOutAsync();
    }
}