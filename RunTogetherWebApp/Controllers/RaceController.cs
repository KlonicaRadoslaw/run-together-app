﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunTogetherWebApp.Data;
using RunTogetherWebApp.Data.Enum;
using RunTogetherWebApp.Extensions;
using RunTogetherWebApp.Interfaces;
using RunTogetherWebApp.Models;
using RunTogetherWebApp.Repositories;
using RunTogetherWebApp.Services;
using RunTogetherWebApp.ViewModels;

namespace RunTogetherWebApp.Controllers
{
    public class RaceController : Controller
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IPhotoService _photoService;

        public RaceController(IRaceRepository raceRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
        {
            _raceRepository = raceRepository;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int category = -1, int page = 1, int pageSize = 6)
        {
            if (page < 1 || pageSize < 1)
            {
                return NotFound();
            }

            // if category is -1 (All) dont filter else filter by selected category
            var races = category switch
            {
                -1 => await _raceRepository.GetSliceAsync((page - 1) * pageSize, pageSize),
                _ => await _raceRepository.GetRacesByCategoryAndSliceAsync((RaceCategory)category, (page - 1) * pageSize, pageSize),
            };

            var count = category switch
            {
                -1 => await _raceRepository.GetCountAsync(),
                _ => await _raceRepository.GetCountByCategoryAsync((RaceCategory)category),
            };

            var viewModel = new IndexRaceViewModel
            {
                Races = races,
                Page = page,
                PageSize = pageSize,
                TotalRaces = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                Category = category,
            };

            return View(viewModel);
        }

        [HttpGet]
        [Route("event/{runningRace}/{id}")]
        public async Task<IActionResult> DetailRace(int id, string runningRace)
        {
            var race = await _raceRepository.GetById(id);
            return race == null ? NotFound() : View(race);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var currentUserId = HttpContext?.User.GetUserId();
            var createRaceViewModel = new CreateRaceViewModel { AppUserId = currentUserId };

            return View(createRaceViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRaceViewModel raceVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(raceVM.Image);

                var race = new Race
                {
                    Title = raceVM.Title,
                    Description = raceVM.Description,
                    Image = result.Url.ToString(),
                    AppUserId = raceVM.AppUserId,
                    RaceCategory = raceVM.RaceCategory,
                    Address = new Address
                    {
                        Street = raceVM.Address.Street,
                        City = raceVM.Address.City,
                        State = raceVM.Address.State,
                    }
                };
                _raceRepository.Add(race);
                return RedirectToAction("Index");
            }
            else
                ModelState.AddModelError("", "Photo upload failed");

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var race = await _raceRepository.GetById(id);
            if (race == null)
                return View("Error");

            var raceVm = new EditRaceViewModel
            {
                Title = race.Title,
                Description = race.Description,
                AddressId = race.AddressId,
                Address = race.Address,
                URL = race.Image,
                RaceCategory = race.RaceCategory
            };
            return View(raceVm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditRaceViewModel raceVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit race");
                return View(raceVM);
            }

            var userRace = await _raceRepository.GetByIdNoTracking(id);

            if (userRace == null)
                return View("Error");

            var photoResult = await _photoService.AddPhotoAsync(raceVM.Image);

            if (photoResult.Error != null)
            {
                ModelState.AddModelError("Image", "Photo upload failed");
                return View(raceVM);
            }

            if (!string.IsNullOrEmpty(userRace.Image))
                _ = _photoService.DeletePhotoAsync(userRace.Image);

            var race = new Race
            {
                Id = id,
                Title = raceVM.Title,
                Description = raceVM.Description,
                Image = photoResult.Url.ToString(),
                AddressId = raceVM.AddressId,
                Address = raceVM.Address,
            };

            _raceRepository.Update(race);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var raceDetails = await _raceRepository.GetById(id);

            if (raceDetails == null)
                return View("Error");

            return View(raceDetails);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteClub(int id)
        {
            var raceDetails = await _raceRepository.GetById(id);

            if (raceDetails == null)
                return View("Error");

            if (!string.IsNullOrEmpty(raceDetails.Image))
                _ = _photoService.DeletePhotoAsync(raceDetails.Image);

            _raceRepository.Delete(raceDetails);

            return RedirectToAction("Index");
        }
    }
}
