using System.Security.Claims;
using week_26_27.ViewModels;

namespace week_26_27.Service.IService;

public interface IProfileService
{
    public Task<ApplicationUser> GetProfile(ApplicationUser applicationUser);

    public Task<Dictionary<string, string>?> ChangePassword(ChangePasswordVM changePasswordVm, ApplicationUser user); 
}
