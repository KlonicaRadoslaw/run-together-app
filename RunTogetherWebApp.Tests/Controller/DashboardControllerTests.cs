using FakeItEasy;
using FluentAssertions;
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
    public class DashboardControllerTests
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly DashboardController _dashboardController;

        public DashboardControllerTests()
        {
            // Arrange
            _dashboardRepository = A.Fake<IDashboardRepository>();
            _dashboardController = new DashboardController(_dashboardRepository);
        }

        [Fact]
        public async Task Index_ReturnsViewWithDashboardViewModel()
        {
            // Arrange
            var userRaces = new List<Race>
        {
            new Race { Title = "Race 1" },
            new Race { Title = "Race 2" }
        };

            var userClubs = new List<Club>
        {
            new Club { Title = "Club 1" },
            new Club { Title = "Club 2" }
        };

            A.CallTo(() => _dashboardRepository.GetAllUserRaces()).Returns(Task.FromResult(userRaces));
            A.CallTo(() => _dashboardRepository.GetAllUserClubs()).Returns(Task.FromResult(userClubs));

            // Act
            var result = await _dashboardController.Index() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();

            var model = result.Model as DashboardViewModel;
            model.Should().NotBeNull();
            model.Races.Should().HaveCount(2);
            model.Clubs.Should().HaveCount(2);
            model.Races[0].Title.Should().Be("Race 1");
            model.Clubs[0].Title.Should().Be("Club 1");

            // Verify that methods were called
            A.CallTo(() => _dashboardRepository.GetAllUserRaces()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _dashboardRepository.GetAllUserClubs()).MustHaveHappenedOnceExactly();
        }
    }
}
