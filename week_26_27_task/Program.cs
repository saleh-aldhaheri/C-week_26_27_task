using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using week_26_27.Helpers;
using week_26_27.Helpers.IHelpers;
using week_26_27.Middlewares;
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
            builder.Services.AddScoped<IRepository<Booking>, Repository<Booking>>();
            builder.Services.AddScoped<IRepository<Ticket>, Repository<Ticket>>();
            builder.Services.AddScoped<IRepository<Cart>, Repository<Cart>>();
            builder.Services.AddScoped<IRepository<CartSeat>, Repository<CartSeat>>();
            builder.Services.AddScoped<IRepository<Auditorium>, Repository<Auditorium>>();
            builder.Services.AddScoped<IRepository<Seat>, Repository<Seat>>();
            builder.Services.AddScoped<IUnitOfWork, UniteOfWork>();

            //services 
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICinemaService, CinemaService>();
            builder.Services.AddScoped<IActorService, ActorService>();
            builder.Services.AddScoped<IMovieService, MovieService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAccountService, AccountSerivce>();

            //helpers
            builder.Services.AddScoped<IFileHelper, LocalFileHelper>();
            builder.Services.AddScoped<IPagination, Pagination>();
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();
            
            builder.Services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
            });


            //Identity 
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                //password
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 0;

                //user
                options.User.RequireUniqueEmail = true;

                //ratelimiting 
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                //Sign In
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


            builder.Services.ConfigureApplicationCookie(option =>
            {
                option.LoginPath = "/identity/account/login";
                option.AccessDeniedPath = "/identity/account/AccessDenied";
            });

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseWhen(
                context =>
                    context.Request.Path.StartsWithSegments("/Identity/Account/Login") ||
                    context.Request.Path.StartsWithSegments("/Identity/Account/Register") ||
                    context.Request.Path.StartsWithSegments("/Identity/Account/SendEmailConfirmation") ||
                    context.Request.Path.StartsWithSegments("/Identity/Account/EmailConfirmation") ||
                    context.Request.Path.StartsWithSegments("/Identity/Account/ForgetPassword") ||
                    context.Request.Path.StartsWithSegments("/Identity/Account/ValidateOtp") ||
                    context.Request.Path.StartsWithSegments("/Identity/Account/NewPassword") ||
                    context.Request.Path.StartsWithSegments("/"),
                branch => { branch.UseMiddleware<GuestMiddleware>(); });

            app.MapStaticAssets();
            
            app.MapControllerRoute(
               name: "default",
               pattern: "{Area=Identity}/{controller=Account}/{action=Login}/{id?}")
               .WithStaticAssets();

            using (var scope = app.Services.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                dbInitializer.Initialize();
            }

            app.Run();
        }
    }
}
