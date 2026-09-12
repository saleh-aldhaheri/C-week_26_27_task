using week_26_27.Repositories.IRepositories;

namespace week_26_27_task.Services;
public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public DashboardVm GetDashboard()
    {
        var moviesCount =  _unitOfWork.movieRepository.Get().Count();
        var actorsCount =  _unitOfWork.actorRepository.Get().Count(); 
        var categoriesCount = _unitOfWork.categryRepository.Get().Count();
        var cinemasCount =  _unitOfWork.cinemaRepository.Get().Count();

        var moviePrices = _unitOfWork.movieRepository.Get()
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
