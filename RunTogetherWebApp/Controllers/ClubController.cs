﻿using Microsoft.AspNetCore.Mvc;
using RunTogetherWebApp.Extensions;
using RunTogetherWebApp.Interfaces;
using RunTogetherWebApp.Models;
using RunTogetherWebApp.ViewModels;

namespace RunTogetherWebApp.Controllers
{
    public class ClubController : Controller
    {
        private readonly IClubRepository _clubRepository;
        private readonly IPhotoService _photoService;
        public ClubController(IClubRepository clubRepository, IPhotoService photoService)
        {
            _clubRepository = clubRepository;
            _photoService = photoService;
        }
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 6)
        {
            if(pageNumber < 1 || pageSize < 1)
                return NotFound();

            var clubs = await _clubRepository.GetSliceAsync((pageNumber - 1) * pageSize, pageSize);
            var count = await _clubRepository.GetCountAsync();

            var clubViewModel = new IndexClubViewModel
            {
                Clubs = clubs,
                PageSize = pageSize,
                PageIndex = pageNumber,
                TotalClubs = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            };

            return View(clubViewModel);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var club = await _clubRepository.GetById(id);

            return club == null ? NotFound() : View(club);
        }

        public async Task<IActionResult> Create()
        {
            var currentUserId = HttpContext.User.GetUserId();
            var createClubViemModel = new CreateClubViewModel
            {
                AppUserId = currentUserId
            };

            return View(createClubViemModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClubViewModel clubVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(clubVM.Image);

                var club = new Club
                {
                    Title = clubVM.Title,
                    Description = clubVM.Description,
                    Image = result.Url.ToString(),
                    AppUserId = clubVM.AppUserId,
                    Address = new Address
                    {
                        Street = clubVM.Address.Street,
                        City = clubVM.Address.City,
                        State = clubVM.Address.State,
                    }
                };
                _clubRepository.Add(club);
                return RedirectToAction("Index");
            }
            else
                ModelState.AddModelError("", "Photo upload failed");

            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            var club = await _clubRepository.GetById(id);
            if (club == null)
                return View("Error");

            var clubVm = new EditClubViewModel
            {
                Title = club.Title,
                Description = club.Description,
                AddressId = club.AddressId,
                Address = club.Address,
                URL = club.Image,
                ClubCategory = club.ClubCategory
            };
            return View(clubVm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditClubViewModel clubVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit club");
                return View("Edit", clubVM);
            }

            var userClub = await _clubRepository.GetByIdNoTracking(id);

            if (userClub != null)
            {
                try
                {
                    await _photoService.DeletePhotoAsync(userClub.Image);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete photo");
                    return View(clubVM);
                }

                var photoResult = await _photoService.AddPhotoAsync(clubVM.Image);

                var club = new Club
                {
                    Id = id,
                    Title = clubVM.Title,
                    Description = clubVM.Description,
                    Image = photoResult.Url.ToString(),
                    AddressId = clubVM.AddressId,
                    Address = clubVM.Address
                };

                _clubRepository.Update(club);

                return RedirectToAction("Index");
            }
            else
            {
                return View(clubVM);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var clubDetails = await _clubRepository.GetById(id);

            if (clubDetails == null)
                return View("Error");

            return View(clubDetails);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteClub(int id)
        {
            var clubDetails = await _clubRepository.GetById(id);

            if (clubDetails == null)
                return View("Error");

            _clubRepository.Delete(clubDetails);

            return RedirectToAction("Index");
        }
    }
}
