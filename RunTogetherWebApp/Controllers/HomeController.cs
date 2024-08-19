using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RunTogetherWebApp.Data;
using RunTogetherWebApp.Helpers;
using RunTogetherWebApp.Interfaces;
using RunTogetherWebApp.Models;
using RunTogetherWebApp.ViewModels;
using System.Diagnostics;
using System.Globalization;
using System.Net;

namespace RunTogetherWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IClubRepository _clubRepository;
        private readonly IOptions<IPInfoSettings> _config;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, 
            IClubRepository clubRepository, IOptions<IPInfoSettings> config,
            SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _clubRepository = clubRepository;
            _config = config;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var ipInfo = new IPInfo();
            var homeViewModel = new HomeViewModel();
            try
            {
                string url = $"https://ipinfo.io?token={_config.Value.Token}";
                var info = new WebClient().DownloadString(url);
                ipInfo = JsonConvert.DeserializeObject<IPInfo>(info);
                RegionInfo myRI1 = new RegionInfo(ipInfo.Country);
                ipInfo.Country = myRI1.EnglishName;
                homeViewModel.City = ipInfo.City;
                homeViewModel.State = ipInfo.Region;
                if(homeViewModel.City != null)
                {
                    homeViewModel.Clubs = await _clubRepository.GetClubByCity(homeViewModel.City);
                }
                else
                {
                    homeViewModel.Clubs = null;
                }
                return View(homeViewModel);

            }
            catch (Exception)
            {
                homeViewModel.Clubs = null;
            }
            return View(homeViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(HomeViewModel homeVM)
        {
            var createVM = homeVM.Register;

            if(!ModelState.IsValid)
                return View(homeVM);

            var user = await _userManager.FindByEmailAsync(createVM.Email);
            if(user != null)
            {
                ModelState.AddModelError("Register.Email", "This email address is already in use");
                return View(homeVM);
            }

            var newUser = new AppUser
            {
                UserName = createVM.UserName,
                Email = createVM.Email
            };

            var newUserResponse = await _userManager.CreateAsync(newUser, createVM.Password);

            if (newUserResponse.Succeeded)
            {
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
            }

            return RedirectToAction("Index", "Club");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
