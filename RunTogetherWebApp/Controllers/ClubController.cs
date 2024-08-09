using Microsoft.AspNetCore.Mvc;
using RunTogetherWebApp.Data;

namespace RunTogetherWebApp.Controllers
{
    public class ClubController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ClubController(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public IActionResult Index()
        {
            var clubs = _context.Clubs.ToList();
            return View(clubs);
        }
    }
}
