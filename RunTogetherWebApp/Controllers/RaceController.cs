using Microsoft.AspNetCore.Mvc;

namespace RunTogetherWebApp.Controllers
{
    public class RaceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
