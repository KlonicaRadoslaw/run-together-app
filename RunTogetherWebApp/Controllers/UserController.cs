using Microsoft.AspNetCore.Mvc;

namespace RunTogetherWebApp.Controllers
{
    public class UserController : Controller
    {
        [HttpGet("users")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
