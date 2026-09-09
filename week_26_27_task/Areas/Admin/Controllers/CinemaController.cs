using Microsoft.AspNetCore.Mvc;

namespace week_26_27_task.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CinemaWithImageVm cinemaWithImage, CancellationToken ct = default)
        {
            if(cinemaWithImage.Image is null || !ModelState.IsValid) 
                 return View(model: cinemaWithImage);

            try
            {
               await _cinemaService.CreateCinema(cinemaWithImage.Cinema, cinemaWithImage.Image, ct);
            
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


            return View(model: new CinemaWithImageVm()
            {
                Cinema = cinema
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromRoute]int Id, CinemaWithImageVm cinemaWithImage, CancellationToken ct = default)
        {
            cinemaWithImage.Cinema.Id = Id; 

            if (!ModelState.IsValid)
                return View(cinemaWithImage);

            try
            {
               await _cinemaService.UpdateCinema(cinemaWithImage.Cinema,  ct, cinemaWithImage.Image);

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
}
