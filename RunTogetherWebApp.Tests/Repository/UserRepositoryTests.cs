using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RunTogetherWebApp.Data;
using RunTogetherWebApp.Models;
using RunTogetherWebApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTogetherWebApp.Tests.Repository
{
    public class UserRepositoryTests
    {
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            if (await databaseContext.Users.CountAsync() < 1)
            {
                databaseContext.Users.Add(
                    new AppUser
                    {
                        Id = "1",
                        UserName = "testuser",
                        Pace = 2,
                        Milage = 100
                    });
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        [Fact]
        public async void UserRepository_Add_ReturnsBool()
        {
            // Arrange
            var user = new AppUser
            {
                Id = "2",
                UserName = "newuser",
                Pace = 2,
                Milage = 50
            };

            var dbContext = await GetDbContext();
            var userRepository = new UserRepository(dbContext);

            // Act
            var result = userRepository.Add(user);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void UserRepository_GetUserById_ReturnsUser()
        {
            // Arrange
            var id = "1";
            var dbContext = await GetDbContext();
            var userRepository = new UserRepository(dbContext);

            // Act
            var result = await userRepository.GetUserById(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<AppUser>();
        }

        [Fact]
        public async void UserRepository_GetAllUsers_ReturnsAllUsers()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var userRepository = new UserRepository(dbContext);

            // Act
            var result = await userRepository.GetAllUsers();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<AppUser>>();
        }
    }
}
