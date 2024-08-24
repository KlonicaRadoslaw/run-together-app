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
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace RunTogetherWebApp.Tests.Controller
{
    public class ClubControllerTests
    {
        private ClubController _clubController;
        private IClubRepository _clubRepository;
        private IPhotoService _photoService;
        public ClubControllerTests()
        {
            //Dependencies
            _clubRepository = A.Fake<IClubRepository>();
            _photoService = A.Fake<IPhotoService>();

            //SUT
            _clubController = new ClubController(_clubRepository, _photoService);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithClubList()
        {
            // Arrange
            var clubs = new List<Club> { new Club(), new Club() };
            A.CallTo(() => _clubRepository.GetSliceAsync(A<int>.Ignored, A<int>.Ignored)).Returns(clubs);
            A.CallTo(() => _clubRepository.GetCountAsync()).Returns(2);

            // Act
            var result = await _clubController.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IndexClubViewModel>(viewResult.Model);
            int count = model.Clubs.Count();
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task Detail_ValidId_ReturnsViewResult_WithClub()
        {
            // Arrange
            var club = new Club { Id = 1, Title = "Test Club" };
            A.CallTo(() => _clubRepository.GetById(1)).Returns(Task.FromResult(club));

            // Act
            var result = await _clubController.DetailClub(1, "Test Club");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Club>(viewResult.Model);
            Assert.Equal(club, model);
        }

        [Fact]
        public async Task Detail_InvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            A.CallTo(() => _clubRepository.GetById(A<int>.Ignored)).Returns(Task.FromResult<Club>(null));

            // Act
            var result = await _clubController.DetailClub(1, "Test Club");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        /*[Fact]
        public async Task Create_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var viewModel = new CreateClubViewModel { Title = "New Club", Image = null };
            var photoUploadResult = new ImageUploadResult { Url = new Uri("http://test.com/photo.jpg") };
            A.CallTo(() => _photoService.AddPhotoAsync(viewModel.Image)).Returns(Task.FromResult(photoUploadResult));

            // Act
            var result = await _clubController.Create(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }*/

        [Fact]
        public async Task Edit_ValidId_ReturnsViewResult_WithEditClubViewModel()
        {
            // Arrange
            var club = new Club { Id = 1, Title = "Test Club", Address = new Address() };
            A.CallTo(() => _clubRepository.GetById(1)).Returns(Task.FromResult(club));

            // Act
            var result = await _clubController.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<EditClubViewModel>(viewResult.Model);
            Assert.Equal("Test Club", model.Title);
        }

        [Fact]
        public async Task Delete_ValidId_ReturnsViewResult_WithClub()
        {
            // Arrange
            var club = new Club { Id = 1, Title = "Test Club" };
            A.CallTo(() => _clubRepository.GetById(1)).Returns(Task.FromResult(club));

            // Act
            var result = await _clubController.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Club>(viewResult.Model);
            Assert.Equal(club, model);
        }

        [Fact]
        public async Task DeleteClub_ValidId_RedirectsToIndex()
        {
            // Arrange
            var club = new Club { Id = 1, Title = "Test Club" };
            A.CallTo(() => _clubRepository.GetById(1)).Returns(Task.FromResult(club));

            // Act
            var result = await _clubController.DeleteClub(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }
    }
}
