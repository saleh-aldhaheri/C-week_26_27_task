using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using week_26_27.Utilities;

namespace week_26_27_task.Areas.Admin.Controllers;

[Area(AreaConstants.ADMIN_AREA)]
[Authorize(Roles = $"{RoleConstants.ADMIN},{RoleConstants.SUPER_ADMIN}")]
public class MovieController : Controller
{
    private IMovieService _movieService;
    private IActorService _actoreService;
    
    public MovieController(
        IMovieService movieService, 
        IActorService actoreService
    )
    {
        _movieService = movieService;
        _actoreService = actoreService;
    }

    public IActionResult Index(MovieWithFilterAndPaginationVM moviesIndex)
    {
        var movies = _movieService.GetMovies(moviesIndex);

        return View(model: movies);
    }

    public IActionResult View(int id)
    {
        var movie = _movieService.GetMovie(id);
        var actorIds = movie.MovieActors.Select(ma => ma.ActorId).ToList(); 
        var actors = _actoreService.GetAllActors(e => actorIds.Contains(e.Id)).ToList(); 

        return View(model: new ViewMovieVm
        {
            Movie = movie,
            Actors = actors
        });
    }

    [HttpGet]
    public IActionResult Create()
    {
        var (cinemasWithAuditoriums,  categories,  actors) =  _movieService.GetDropDowns();

        return View(new MovieWithCategoriesCinemasActors()
        {
            Categories = categories,
            CinemasWithAuditoriums = cinemasWithAuditoriums,
            Actors = actors
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MovieWithCategoriesCinemasActors movieWithResource, CancellationToken ct = default)
    {
        if (movieWithResource.Image is null || !ModelState.IsValid)
        {
            (movieWithResource.CinemasWithAuditoriums, movieWithResource.Categories, movieWithResource.Actors) =  _movieService.GetDropDowns();

            return View(movieWithResource);
        }

        try
        {
            await _movieService.CreateMovie(
              movieWithResource,
                ct
            );
        }
        catch (Exception)
        {
            return BadRequest();
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Movie created successfully!";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Update([FromRoute] int id)
    {
        Movie? movie = null;

        var (cinemasWithAuditoriums, categories, actors) =  _movieService.GetDropDowns();

        try
        {
            movie = _movieService.GetMovie(id);

        }
        catch (Exception)
        {
            return NotFound();
        }

        return View(new MovieWithCategoriesCinemasActors()
        {
            Movie = movie,
            Categories = categories,
            CinemasWithAuditoriums = cinemasWithAuditoriums,
            Actors = actors,
            SelectedAuditoriumId = movie.AuditoriumId,
            SelectedCategroyId = movie.CategoryId,
            SelectedCinemaId = movie.Auditorium.CinemaId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, MovieWithCategoriesCinemasActors movieWithResource, CancellationToken ct = default)
    {
        movieWithResource.Movie.Id = id;

        if (!ModelState.IsValid)
        {
            (movieWithResource.CinemasWithAuditoriums, movieWithResource.Categories, movieWithResource.Actors) = _movieService.GetDropDowns();

            var dbMovie = _movieService.GetMovie(id);
            movieWithResource.Movie.MainImg = dbMovie.MainImg;
            movieWithResource.Movie.MovieSubImgs = dbMovie.MovieSubImgs;
            movieWithResource.Movie.MovieActors = dbMovie.MovieActors;

            return View(movieWithResource);
        }

        try
        {
            await _movieService.UpdateMovie(
                movieWithResource,
                ct
                );
        }
        catch (Exception)
        {
            return BadRequest();
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Movie updated successfully!";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        try
        {
           await _movieService.DeleteMovie(id);

        }catch(Exception)
        {
            return BadRequest();
        }

        TempData[key: NotificationConstants.SUCCESS_NOTIFICATION] = "Movie deleted successfully!";

        return RedirectToAction(nameof(Index)); 
    }
}
