using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RunTogetherWebApp.Controllers;
using RunTogetherWebApp.Interfaces;
using RunTogetherWebApp.Models;
using RunTogetherWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTogetherWebApp.Tests.Controller
{
    public class RaceControllerTests
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IPhotoService _photoService;
        private readonly RaceController _raceController;

        public RaceControllerTests()
        {
            _raceRepository = A.Fake<IRaceRepository>();
            _photoService = A.Fake<IPhotoService>();
            var httpContextAccessor = A.Fake<IHttpContextAccessor>();
            _raceController = new RaceController(_raceRepository, _photoService, httpContextAccessor);
        }

        [Fact]
        public async Task Index_ShouldReturnCorrectViewModel_WhenCategoryIsAll()
        {
            // Arrange
            var races = new List<Race>
        {
            new Race { Id = 1, Title = "Race1" },
            new Race { Id = 2, Title = "Race2" }
        };

            A.CallTo(() => _raceRepository.GetSliceAsync(0, 6)).Returns(Task.FromResult((IEnumerable<Race>)races));
            A.CallTo(() => _raceRepository.GetCountAsync()).Returns(Task.FromResult(2));

            // Act
            var result = await _raceController.Index(-1, 1, 6) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as IndexRaceViewModel;
            model.Should().NotBeNull();
            model.Races.Should().HaveCount(2);
            model.TotalRaces.Should().Be(2);
            model.Page.Should().Be(1);
            model.TotalPages.Should().Be(1);
        }

        [Fact]
        public async Task DetailRace_ShouldReturnNotFound_WhenRaceDoesNotExist()
        {
            // Arrange
            A.CallTo(() => _raceRepository.GetById(A<int>.Ignored)).Returns(Task.FromResult<Race>(null));

            // Act
            var result = await _raceController.DetailRace(1, "some-race");

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreatePost_ShouldRedirectToIndex_WhenModelStateIsValid()
        {
            // Arrange
            var raceVm = new CreateRaceViewModel
            {
                Title = "Race 1",
                Description = "Test race",
                Image = A.Fake<IFormFile>(),
                Address = new Address { City = "Test City", State = "Test State" },
                AppUserId = "User1"
            };

            var photoUploadResult = new CloudinaryDotNet.Actions.ImageUploadResult { Url = new Uri("https://someurl.com") };
            A.CallTo(() => _photoService.AddPhotoAsync(A<IFormFile>.Ignored)).Returns(photoUploadResult);

            // Act
            var result = await _raceController.Create(raceVm) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            A.CallTo(() => _raceRepository.Add(A<Race>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public async Task Edit_ShouldReturnViewWithModel_WhenRaceExists()
        {
            // Arrange
            var race = new Race { Id = 1, Title = "Race 1", Image = "imageUrl" };
            A.CallTo(() => _raceRepository.GetById(1)).Returns(Task.FromResult(race));

            // Act
            var result = await _raceController.Edit(1) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result.Model as EditRaceViewModel;
            model.Should().NotBeNull();
            model.Title.Should().Be("Race 1");
            model.URL.Should().Be("imageUrl");
        }

        [Fact]
        public async Task Delete_ShouldRedirectToIndex_WhenRaceIsDeleted()
        {
            // Arrange
            var race = new Race { Id = 1, Image = "someImageUrl" };
            A.CallTo(() => _raceRepository.GetById(1)).Returns(Task.FromResult(race));

            // Act
            var result = await _raceController.DeleteClub(1) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
            A.CallTo(() => _photoService.DeletePhotoAsync("someImageUrl")).MustHaveHappened();
            A.CallTo(() => _raceRepository.Delete(A<Race>.Ignored)).MustHaveHappened();
        }
    }
}
