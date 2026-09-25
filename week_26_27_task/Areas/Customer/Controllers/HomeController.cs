using Microsoft.AspNetCore.Mvc;
using week_26_27.Utilities;

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
