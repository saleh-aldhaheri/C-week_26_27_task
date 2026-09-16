using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using week_26_27_task.Models;
using week_26_27_task.Services;

namespace week_26_27_task.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public HomeController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity is null || !User.Identity.IsAuthenticated)
                return RedirectToAction("Login", ControllerConstants.ACCONT_CONTROLLER, new
                {
                    area = AreaConstants.IDENTITY_AREA
                });

            var vm =  _dashboardService.GetDashboard();
            return View(vm);
        }

        public IActionResult Privacy()
        {
            if (User.Identity is null || !User.Identity.IsAuthenticated)
                return RedirectToAction("Login", ControllerConstants.ACCONT_CONTROLLER, new
                {
                    area = AreaConstants.IDENTITY_AREA
                });

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            if (User.Identity is null || !User.Identity.IsAuthenticated)
                return RedirectToAction("Login", ControllerConstants.ACCONT_CONTROLLER, new
                {
                    area = AreaConstants.IDENTITY_AREA
                });

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
