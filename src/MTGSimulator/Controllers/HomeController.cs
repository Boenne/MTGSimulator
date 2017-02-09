using Microsoft.AspNetCore.Mvc;

namespace MTGSimulator.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}