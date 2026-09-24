namespace week_26_27_task.Service.IService;

public interface IMovieService
{
    MovieWithFilterAndPaginationVM GetMovies(MovieWithFilterAndPaginationVM MoviesIndex);

    Task CreateMovie(MovieWithCategoriesCinemasActors movieWithResouce, CancellationToken ct = default);

    Task UpdateMovie(MovieWithCategoriesCinemasActors movieWithResource, CancellationToken ct = default);

    Task DeleteMovie(int id, CancellationToken ct = default);

    (IQueryable<Cinema>, IQueryable<Category>, IQueryable<Actor>) GetDropDowns();

    Movie GetMovie(int id);
}
