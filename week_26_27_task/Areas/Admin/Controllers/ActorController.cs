using Microsoft.AspNetCore.Mvc;

namespace week_26_27_task.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        private IActorService _actorService;

        public ActorController(IActorService actorService)
        {
            _actorService = actorService;
        }

        public IActionResult Index(ActorWithFilterAndPaginationVM actorsIndex)
        {
            var actors = _actorService.GetActors(actorsIndex);
            return View(model: actors);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActorWithImageVm actorWithImage, CancellationToken ct = default)
        {
            if (actorWithImage.Image is null || !ModelState.IsValid)
                return View(model: actorWithImage);

            try
            {
                await _actorService.CreateActor(actorWithImage.Actor, actorWithImage.Image, ct);

            }
            catch (Exception)
            {
                return BadRequest();
            }

            TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Actor created successfully!";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update([FromRoute] int id)
        {
            Actor? actor = null;

            try
            {
                actor = _actorService.GetActor(id);
            }
            catch (Exception)
            {
                return NotFound();
            }

            return View(model: new ActorWithImageVm()
            {
                Actor = actor
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ActorWithImageVm actorWithImage, CancellationToken ct = default)
        {
            actorWithImage.Actor.Id = id;

            if (!ModelState.IsValid)
                return View(actorWithImage);

            try
            {
                await _actorService.UpdateActor(actorWithImage.Actor, ct, actorWithImage.Image);

            }
            catch (Exception)
            {
                return BadRequest();
            }

            TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Actor updated successfully!";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct = default)
        {
            try
            {
                await _actorService.DeleteActor(id, ct);

            }
            catch (Exception)
            {
                BadRequest();
            }

            TempData[key: NotificationConstants.SUCCESS_NOTIFICATION] = "Actor deleted successfully!";

            return RedirectToAction(nameof(Index));
        }
    }
}
