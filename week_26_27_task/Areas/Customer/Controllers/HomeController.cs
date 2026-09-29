using Microsoft.AspNetCore.Mvc;

namespace week_26_27.Areas.Customer.Controllers
{
    [Area(AreaConstants.CUSTOMER_AREA)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
