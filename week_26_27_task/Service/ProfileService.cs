using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using week_26_27.Service.IService;
using week_26_27.ViewModels;

namespace week_26_27.Service;

public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ApplicationUser> GetProfile(ApplicationUser applicationUser)
    {
        var user = await _userManager.FindByIdAsync(applicationUser.Id);
        
        if (user is null)
            throw new Exception(); 

        user.Email = applicationUser.Email;
        user.PhoneNumber = applicationUser.PhoneNumber;
        user.FirstName = applicationUser.FirstName;
        user.LastName = applicationUser.LastName;

        await _userManager.UpdateAsync(user);
        
        return user;
    }

    public async Task<Dictionary<string, string>?> ChangePassword(ChangePasswordVM changePasswordVm, ApplicationUser user)
    {
        var result = await _userManager.ChangePasswordAsync(user, changePasswordVm.OldPassword, changePasswordVm.NewPassword);
        var errors = new Dictionary<string, string>();

        if (!result.Succeeded)
        {
            foreach(var error in result.Errors)
                errors.Add(error.Description, error.Description);

            return errors; 
        }

        return null;
    }
}
