using System.Linq.Expressions;

namespace week_26_27_task.Service.IService;
 public interface ICinemaService
{
     CinemaWithFilterAndPaginationVM GetCinemas(CinemaWithFilterAndPaginationVM CinemasIndex);

     Task CreateCinema(Cinema cinema,IFormFile image, CancellationToken ct = default);

     Task UpdateCinema(Cinema cinema, CancellationToken ct = default, IFormFile? image = null);

     Task DeleteCinema(int id, CancellationToken ct = default);

     Cinema GetCinema(int id);

     IQueryable<Cinema> GetAllCinemas(Expression<Func<Cinema, bool>>? exprission = null);
}
