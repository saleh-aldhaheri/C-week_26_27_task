using Microsoft.EntityFrameworkCore;
using week_26_27_task.Models;
using week_26_27_task.ViewModels;

namespace week_26_27_task.Services;
public class DashboardService : IDashboardService
{
    private readonly IRepository<Movie> _movieRepository;
    private readonly IRepository<Actor> _actorRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Cinema> _cinemaRepository;

    public DashboardService(
        IRepository<Movie> movieRepository,
        IRepository<Actor> actorRepository,
        IRepository<Category> categoryRepository,
        IRepository<Cinema> cinemaRepository)
    {
        _movieRepository = movieRepository;
        _actorRepository = actorRepository;
        _categoryRepository = categoryRepository;
        _cinemaRepository = cinemaRepository;
    }

    public DashboardVm GetDashboard()
    {
        var moviesCount =  _movieRepository.Get().Count();
        var actorsCount =  _actorRepository.Get().Count(); 
        var categoriesCount = _categoryRepository.Get().Count();
        var cinemasCount =  _cinemaRepository.Get().Count();

        var moviePrices = _movieRepository.Get()
            .OrderBy(m => m.StartAt)
            .Select(m => new MoviePricePointVm
            {
                Title = m.Title,
                StartAt = m.StartAt,
                Price = m.Price
            })
            .ToList();

        return new DashboardVm
        {
            MoviesCount = moviesCount,
            ActorsCount = actorsCount,
            CategoriesCount = categoriesCount,
            CinemasCount = cinemasCount,
            MoviePrices = moviePrices
        };
    }
}
