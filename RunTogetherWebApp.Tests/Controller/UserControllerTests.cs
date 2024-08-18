using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RunTogetherWebApp.Controllers;
using RunTogetherWebApp.Interfaces;
using RunTogetherWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTogetherWebApp.Tests.Controller
{
    public class UserControllerTests
    {
        private UserController _userController;
        private IUserRepository _userRepository;

        public UserControllerTests()
        {
            // Dependencies
            _userRepository = A.Fake<IUserRepository>();

            // SUT
            _userController = new UserController(_userRepository);
        }

        [Fact]
        public async Task UserController_Index_ReturnsSuccess()
        {
            // Arrange
            var users = A.Fake<IEnumerable<AppUser>>();
            A.CallTo(() => _userRepository.GetAllUsers()).Returns(users);

            // Act
            var result = await _userController.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task UserController_Detail_ReturnsSuccess()
        {
            // Arrange
            var id = "1";
            var user = A.Fake<AppUser>();
            A.CallTo(() => _userRepository.GetUserById(id)).Returns(user);

            // Act
            var result = await _userController.Detail(id);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
    }
}
