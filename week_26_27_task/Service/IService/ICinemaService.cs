using System.Linq.Expressions;

namespace week_26_27_task.Service.IService;
 public interface ICinemaService
{
     CinemaWithFilterAndPaginationVM GetCinemas(CinemaWithFilterAndPaginationVM CinemasIndex);

     Task CreateCinema(CinemaWithResources cinemaWithResources, CancellationToken ct = default);

     Task UpdateCinema(CinemaWithResources cinemaWithResource, CancellationToken ct = default);

     Task DeleteCinema(int id, CancellationToken ct = default);

     Cinema GetCinema(int id);

     IQueryable<Cinema> GetAllCinemas(Expression<Func<Cinema, bool>>? exprission = null);
}
