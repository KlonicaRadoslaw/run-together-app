using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using RunTogetherWebApp.Controllers;
using RunTogetherWebApp.Data;
using RunTogetherWebApp.Models;
using RunTogetherWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTogetherWebApp.Tests.Controller
{
    public class AccountControllerTests
    {
        private AccountController _accountController;
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        private ApplicationDbContext _context;

        public AccountControllerTests()
        {
            // In-memory database setup
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();
            if (!_context.Users.Any())
            {
                _context.Users.Add(
                    new AppUser
                    {
                        Id = "1",
                        UserName = "testuser",
                        Email = "test@test.com",
                        Pace = 2,
                        Milage = 100
                    });
                _context.SaveChanges();
            }

            // Fake UserManager and SignInManager setup
            _userManager = A.Fake<UserManager<AppUser>>(options => options.WithArgumentsForConstructor(new object[]
            {
            A.Fake<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null
            }));

            _signInManager = A.Fake<SignInManager<AppUser>>(options => options.WithArgumentsForConstructor(new object[]
            {
            _userManager,
            A.Fake<IHttpContextAccessor>(),
            A.Fake<IUserClaimsPrincipalFactory<AppUser>>(),
            null, null, null, null
            }));

            // TempData setup
            var tempData = A.Fake<ITempDataDictionary>();
            _accountController = new AccountController(_userManager, _signInManager, _context)
            {
                TempData = tempData
            };
        }

        [Fact]
        public void AccountController_LoginGet_ReturnsView()
        {
            // Act
            var result = _accountController.Login();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task AccountController_LoginPost_InvalidModelState_ReturnsView()
        {
            // Arrange
            var loginViewModel = new LoginViewModel();
            _accountController.ModelState.AddModelError("EmailAddress", "Required");

            // Act
            var result = await _accountController.Login(loginViewModel);

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().Model.Should().Be(loginViewModel);
        }

        [Fact]
        public async Task AccountController_LoginPost_ValidCredentials_RedirectsToIndex()
        {
            // Arrange
            var loginViewModel = new LoginViewModel { EmailAddress = "test@test.com", Password = "password" };
            var user = A.Fake<AppUser>();

            A.CallTo(() => _userManager.FindByEmailAsync(loginViewModel.EmailAddress)).Returns(user);
            A.CallTo(() => _userManager.CheckPasswordAsync(user, loginViewModel.Password)).Returns(true);
            A.CallTo(() => _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false))
                .Returns(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await _accountController.Login(loginViewModel);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectResult = result as RedirectToActionResult;
            redirectResult.ActionName.Should().Be("Index");
            redirectResult.ControllerName.Should().Be("Race");
        }

        [Fact]
        public async Task AccountController_LoginPost_InvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            var loginViewModel = new LoginViewModel { EmailAddress = "test@test.com", Password = "wrongpassword" };
            var user = A.Fake<AppUser>();

            A.CallTo(() => _userManager.FindByEmailAsync(loginViewModel.EmailAddress)).Returns(user);
            A.CallTo(() => _userManager.CheckPasswordAsync(user, loginViewModel.Password)).Returns(false);

            // Act
            var result = await _accountController.Login(loginViewModel);

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().Model.Should().Be(loginViewModel);
            _accountController.TempData["Error"].Should().Be("Wrong credentials. Please try again");
        }

        [Fact]
        public void AccountController_RegisterGet_ReturnsView()
        {
            // Act
            var result = _accountController.Register();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task AccountController_RegisterPost_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var registerViewModel = new RegisterViewModel { EmailAddress = "test@test.com", Password = "password" };

            A.CallTo(() => _userManager.FindByEmailAsync(registerViewModel.EmailAddress)).Returns((AppUser)null);
            A.CallTo(() => _userManager.CreateAsync(A<AppUser>.Ignored, registerViewModel.Password))
                .Returns(IdentityResult.Success);

            // Act
            var result = await _accountController.Register(registerViewModel);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectResult = result as RedirectToActionResult;
            redirectResult.ActionName.Should().Be("Index");
            redirectResult.ControllerName.Should().Be("Race");
        }

        [Fact]
        public async Task AccountController_RegisterPost_EmailAlreadyExists_ReturnsViewWithError()
        {
            // Arrange
            var registerViewModel = new RegisterViewModel { EmailAddress = "test@test.com", Password = "password" };
            var existingUser = A.Fake<AppUser>();

            A.CallTo(() => _userManager.FindByEmailAsync(registerViewModel.EmailAddress)).Returns(existingUser);

            // Act
            var result = await _accountController.Register(registerViewModel);

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().Model.Should().Be(registerViewModel);
            _accountController.TempData["Error"].Should().Be("This email address is already in use");
        }

        [Fact]
        public async Task AccountController_Logout_RedirectsToHomeIndex()
        {
            // Act
            var result = await _accountController.Logout();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectResult = result as RedirectToActionResult;
            redirectResult.ActionName.Should().Be("Index");
            redirectResult.ControllerName.Should().Be("Home");
        }
    }

}
