using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
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

            //services 
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICinemaService, CinemaService>();
            builder.Services.AddScoped<IActorService, ActorService>();
            builder.Services.AddScoped<IMovieService, MovieService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();

            //helpers
            builder.Services.AddScoped<IFileHelper, LocalFileHelper>();
            builder.Services.AddScoped<IPagination, Pagination>(); 

            builder.Services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
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
