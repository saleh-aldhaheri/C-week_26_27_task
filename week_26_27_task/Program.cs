using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using week_26_27.Models;
using week_26_27.Repositories;
using week_26_27.Repositories.IRepositories;
using week_26_27.Service;
using week_26_27.Service.IService;
using week_26_27_task.Data.ApplicationDbContext;
using week_26_27_task.Services;

namespace week_26_27_task
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //repositories
            builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
            builder.Services.AddScoped<IRepository<Cinema>, Repository<Cinema>>();
            builder.Services.AddScoped<IRepository<Actor>, Repository<Actor>>();
            builder.Services.AddScoped<IRepository<Movie>, Repository<Movie>>();
            builder.Services.AddScoped<IBulkRepository<MovieActor>, BulkRepository<MovieActor>>();
            builder.Services.AddScoped<IBulkRepository<MovieSubImg>, BulkRepository<MovieSubImg>>();
            builder.Services.AddScoped<IRepository<ApplicationUserOtp>, Repository<ApplicationUserOtp>>();
            builder.Services.AddScoped<IUnitOfWork, UniteOfWork>();

            //services 
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICinemaService, CinemaService>();
            builder.Services.AddScoped<IActorService, ActorService>();
            builder.Services.AddScoped<IMovieService, MovieService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IAccountService<ApplicationUser>, AccountService<ApplicationUser>>();
            
            //helpers
            builder.Services.AddScoped<IFileHelper, LocalFileHelper>();
            builder.Services.AddScoped<IPagination, Pagination>(); 

            builder.Services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
            });
            
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddTransient<IEmailSender, EmailSenderService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
                app.MapControllerRoute(
                   name: "default",
                   pattern: "{Area=Admin}/{controller=Home}/{action=Index}/{id?}")
                   .WithStaticAssets();

            app.Run();
        }
    }
}
