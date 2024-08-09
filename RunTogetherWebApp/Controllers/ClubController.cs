using Microsoft.AspNetCore.Mvc;

namespace RunTogetherWebApp.Controllers
{
    public class ClubController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
