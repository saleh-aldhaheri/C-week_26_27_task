namespace week_26_27_task.Service.IService;

public interface IMovieService
{
    MovieWithFilterAndPaginationVM GetMovies(MovieWithFilterAndPaginationVM MoviesIndex);

    Task CreateMovie(Movie movie, IFormFile image, List<IFormFile>? images,List<int>? actorsIds, CancellationToken ct = default);

    Task UpdateMovie(Movie movie, IFormFile? image, List<IFormFile>? images, List<int>? actorsIds, CancellationToken ct = default);

    Task DeleteMovie(int id, CancellationToken ct = default);

    (IQueryable<Cinema>, IQueryable<Category>, IQueryable<Actor>) GetDropDowns();

    Movie GetMovie(int id);
}
