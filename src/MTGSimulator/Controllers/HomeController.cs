using Microsoft.AspNetCore.Mvc;

namespace MTGSimulator.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Draft()
        {
            return View();
        }
    }
}