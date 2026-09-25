using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using week_26_27.Utilities;

namespace week_26_27_task.Areas.Admin.Controllers;

[Area(AreaConstants.ADMIN_AREA)]
[Authorize(Roles = $"{RoleConstants.ADMIN},{RoleConstants.SUPER_ADMIN}")]
public class CinemaController : Controller
{
    private ICinemaService _cinemaService;

    public CinemaController(ICinemaService cinemaService)
    {
        _cinemaService = cinemaService;
    }
    
    public IActionResult Index(CinemaWithFilterAndPaginationVM cinemasIndex)
    {
        var cinemas = _cinemaService.GetCinemas(cinemasIndex);
        return View(model: cinemas);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(model: new CinemaWithResources());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CinemaWithResources cinemaWithResouces, CancellationToken ct = default)
    {
        if (cinemaWithResouces.Image is null)
            ModelState.AddModelError(nameof(cinemaWithResouces.Image), "The cinema image is required.");

        if (cinemaWithResouces.CreateAuditorium is null || !cinemaWithResouces.CreateAuditorium.Any())
            ModelState.AddModelError(string.Empty, "Please add at least one auditorium.");

        if (!ModelState.IsValid)
            return View(model: cinemaWithResouces);

        try
        {
           await _cinemaService.CreateCinema(cinemaWithResouces, ct);
        
        }catch(Exception)
        {
            return BadRequest(); 
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Cinema created successfully!";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Update([FromRoute] int id)
    {
        Cinema? cinema = null;

        try
        {
            cinema = _cinemaService.GetCinema(id);
        }
        catch (Exception)
        {
            return NotFound();
        }


        return View(model: new CinemaWithResources()
        {
            Cinema = cinema
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update([FromRoute]int Id, CinemaWithResources cinemaWithResource, CancellationToken ct = default)
    {
        cinemaWithResource.Cinema.Id = Id; 

        if (!ModelState.IsValid)
            return View(cinemaWithResource);

        try
        {
           await _cinemaService.UpdateCinema(cinemaWithResource, ct);

        }catch(Exception)
        {
            return BadRequest();
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Cinema updated successfully!";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete([FromRoute]int id, CancellationToken ct = default)
    {
        try
        {
            await _cinemaService.DeleteCinema(id, ct);

        }catch(Exception)
        {
           BadRequest();
        }

        TempData[key: NotificationConstants.SUCCESS_NOTIFICATION] = "Cinema deleted successfully!";

        return RedirectToAction(nameof(Index));
    }
}
