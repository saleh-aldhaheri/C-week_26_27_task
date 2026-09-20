using Microsoft.AspNetCore.Identity;
using week_26_27.ViewModels;

namespace week_26_27.Service.IService;

public interface IUserService
{
    public Task<UserWithFilterAndPaginationVM> GetUsers(UserWithFilterAndPaginationVM userIndex);
    public Task ChangeRole(ChangeRoleVM chanageRoleVm);
    public Task<UserWithRoleVM> updateGet(string id);
    public Task<UserWithRoleVM?> UpdateUser(UserWithRoleVM userWithRoleVm);
    public Task Delete(string id);
}
