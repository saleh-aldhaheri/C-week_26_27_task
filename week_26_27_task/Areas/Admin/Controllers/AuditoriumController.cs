using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using week_26_27.Service.IService;
using week_26_27.Utilities;

namespace week_26_27_task.Areas.Admin.Controllers;

[Area(AreaConstants.ADMIN_AREA)]
[Authorize(Roles = $"{RoleConstants.ADMIN},{RoleConstants.SUPER_ADMIN}")]
public class AuditoriumController : Controller
{
    private readonly ICinemaService _cinemaService;
    private readonly IAuditoriumService _auditoriumService;
    private readonly ILogger<AuditoriumController> _logger;

    public AuditoriumController(ICinemaService cinemaService, IAuditoriumService auditoriumService, ILogger<AuditoriumController> logger)
    {
        _cinemaService = cinemaService;
        _auditoriumService = auditoriumService;
        _logger = logger;
    }

    public IActionResult Index(int cinemaId)
    {
        try
        {
            var cinema = _cinemaService.GetCinema(cinemaId);

            return View(model: cinema);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load cinema {CinemaId} for auditorium management.", cinemaId);
            return NotFound();
        }
    }

    [HttpGet]
    public IActionResult Update(int id)
    {
        var auditorium = _auditoriumService.GetAuditorim(id);

        if (auditorium is null)
            return NotFound();

        return View(model: new UpdateAuditoriumVM
        {
            Id = auditorium.Id,
            CinemaId = auditorium.CinemaId,
            Name = auditorium.Name,
            Rows = auditorium.Rows, 
            Columns = auditorium.Columns
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(UpdateAuditoriumVM updateAuditoriumVm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(updateAuditoriumVm);

        var auditorium = _auditoriumService.GetAuditorim(updateAuditoriumVm.Id);

        if (auditorium is null)
            return NotFound();

        try
        {
            var errors  = await _auditoriumService.UpdateAuditorium(updateAuditoriumVm, ct);

            if(errors is not null)
            {
                foreach (var error in errors)
                    TempData[error.Key] = error.Value;

                return View(updateAuditoriumVm);
            }
        }catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to update auditorium {AuditoriumId}.", updateAuditoriumVm.Id);
            throw;
        }

        return RedirectToAction(nameof(Index),  new
        {
            cinemaId = auditorium.CinemaId
        });
    }

    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var auditorium = _auditoriumService.GetAuditorim(id);

        if (auditorium is null)
            return NotFound();

        var cinemaId = auditorium.CinemaId;

        try
        {
            var errors = await _auditoriumService.DeleteAuditorium(id, ct);

            if(errors is not null) 
                foreach (var error in errors)
                    TempData[error.Key] = error.Value;

        }catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to delete auditorium {AuditoriumId}.", id);
            throw;
        }

        return RedirectToAction(nameof(Index), new
        {
            cinemaId = cinemaId
        });
    }
}
