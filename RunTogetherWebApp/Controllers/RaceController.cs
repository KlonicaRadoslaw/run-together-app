using Microsoft.AspNetCore.Mvc;
using RunTogetherWebApp.Data;

namespace RunTogetherWebApp.Controllers
{
    public class RaceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RaceController(ApplicationDbContext context) 
        { 
            _context = context; 
        }
        public IActionResult Index()
        {
            var races = _context.Races.ToList();
            return View(races);
        }
    }
}
