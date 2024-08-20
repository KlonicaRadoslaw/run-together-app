using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using RunTogetherWebApp.Extensions;
using RunTogetherWebApp.Interfaces;
using RunTogetherWebApp.Models;
using RunTogetherWebApp.ViewModels;

namespace RunTogetherWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IPhotoService _photoService;

        public DashboardController(IDashboardRepository dashboardRepository,
                                   IPhotoService photoService)
        {
            _dashboardRepository = dashboardRepository;
            _photoService = photoService;
        }

        private void MapUserEdit(AppUser user, EditUserDashboardViewModel editVM, ImageUploadResult photoResult)
        {
            user.Id = editVM.Id;
            user.Pace = editVM.Pace;
            user.Milage = editVM.Mileage;
            user.ProfileImageUrl = photoResult.Url.ToString();
            user.City = editVM.City;
            user.State = editVM.State;
        }

        public async Task<IActionResult> Index()
        {
            var userRaces = await _dashboardRepository.GetAllUserRaces();
            var userClubs = await _dashboardRepository.GetAllUserClubs();
            var dashboardViewModel = new DashboardViewModel()
            {
                Races = userRaces,
                Clubs = userClubs
            };
            return View(dashboardViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> EditUserProfile()
        {
            var currentUserId = HttpContext?.User.GetUserId();
            var user = await _dashboardRepository.GetUserById(currentUserId);

            if (user == null)
                return View("Error");

            var editUserViewModel = new EditUserDashboardViewModel()
            {
                Id = currentUserId,
                Pace = user.Pace,
                Mileage = user.Milage,
                ProfileImageUrl = user.ProfileImageUrl,
                City = user.City,
                State = user.State
            };

            return View(editUserViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserProfile(EditUserDashboardViewModel editVM)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit profile");
                return View("EditUserProfile", editVM);
            }

            var user = await _dashboardRepository.GetByIdNoTracking(editVM.Id);

            if (user == null)
                return View("Error");

            var photoResult = await _photoService.AddPhotoAsync(editVM.Image);

            if (photoResult.Error != null)
            {
                ModelState.AddModelError("Image", "Failed to upload image");
                return View("EditUserProfile", editVM);
            }

            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                _ = _photoService.DeletePhotoAsync(user.ProfileImageUrl);

            MapUserEdit(user, editVM, photoResult);

            _dashboardRepository.Update(user);

            return RedirectToAction("Index");
        }
    }
}
