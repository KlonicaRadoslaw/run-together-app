using Microsoft.AspNetCore.Mvc;

namespace RunTogetherWebApp.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
