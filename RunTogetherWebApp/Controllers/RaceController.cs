using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunTogetherWebApp.Data;
using RunTogetherWebApp.Interfaces;
using RunTogetherWebApp.Models;
using RunTogetherWebApp.Repositories;

namespace RunTogetherWebApp.Controllers
{
    public class RaceController : Controller
    {
        private readonly IRaceRepository _raceRepository;

        public RaceController(IRaceRepository raceRepository)
        {
            _raceRepository = raceRepository;
        }
        public async Task<IActionResult> Index()
        {
            var races = await _raceRepository.GetAll();
            return View(races);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var race = await _raceRepository.GetById(id);
            return View(race);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Race race)
        {
            if (!ModelState.IsValid)
                return View(race);

            _raceRepository.Add(race);

            return RedirectToAction("Index");
        }
    }
}
