using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using week_26_27.Helpers.IHelpers;
using week_26_27.Models;
using week_26_27.Utilities;
using week_26_27_task.Data.ApplicationDbContext;

namespace week_26_27.Helpers;

public class DbInitializer : IDbInitializer
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _dbContext;

    public DbInitializer(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public void Initialize()
    {
        migrate();
        SeedRoles();
        SeedAdmin();
    }

    private void migrate()
    {
        if (_dbContext.Database.GetPendingMigrations().Any())
            _dbContext.Database.Migrate();
    }

    private void SeedRoles()
    {
        if (_roleManager.Roles.Any())
            return;

        _roleManager.CreateAsync(new IdentityRole
        {
            Name = RoleConstants.SUPER_ADMIN
        })
            .GetAwaiter()
            .GetResult();

        _roleManager.CreateAsync(new IdentityRole
        {
            Name = RoleConstants.ADMIN
        })
            .GetAwaiter()
            .GetResult();
    }

    private void SeedAdmin()
    {
        var superAdmin = _userManager.GetUsersInRoleAsync(RoleConstants.SUPER_ADMIN).GetAwaiter().GetResult();

        if (superAdmin is not null && superAdmin.Any())
            return;

        var user = new ApplicationUser
        {
            FirstName = "Super",
            LastName = "Admin",
            Email = "SuperAdmin@gmail.com",
            UserName = "super_admin",
            EmailConfirmed = true
        };

        _userManager.CreateAsync(user, "P@ssword123")
            .GetAwaiter()
            .GetResult();

        _userManager.AddToRoleAsync(user, RoleConstants.SUPER_ADMIN)
            .GetAwaiter()
            .GetResult();
    }
}