using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RunTogetherWebApp.Data;
using RunTogetherWebApp.Models;
using RunTogetherWebApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RunTogetherWebApp.Tests.Repository
{
    public class DashboardRepositoryTests
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardRepositoryTests()
        {
            _httpContextAccessor = A.Fake<IHttpContextAccessor>();
        }

        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dbContext = new ApplicationDbContext(options);
            dbContext.Database.EnsureCreated();

            // Check if clubs or races already exist
            if (!await dbContext.Clubs.AnyAsync() && !await dbContext.Races.AnyAsync())
            {
                var appUser1 = new AppUser { Id = "user1" };
                var appUser2 = new AppUser { Id = "user2" };

                // Seed Clubs
                dbContext.Clubs.AddRange(new List<Club>
        {
            new Club { Title = "Club 1", AppUser = appUser1 },
            new Club { Title = "Club 2", AppUser = appUser2 }
        });

                // Seed Races
                dbContext.Races.AddRange(new List<Race>
        {
            new Race { Title = "Race 1", Description = "Desc 1", AppUser = appUser1 },
            new Race { Title = "Race 2", Description = "Desc 2", AppUser = appUser2 }
        });

                await dbContext.SaveChangesAsync();
            }

            return dbContext;
        }

        [Fact]
        public async Task GetAllUserClubs_ReturnsUserClubs()
        {
            // Arrange
            var dbContext = await GetDbContext();

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, "user1")
            }));

            A.CallTo(() => _httpContextAccessor.HttpContext.User).Returns(claimsPrincipal);

            var dashboardRepository = new DashboardRepository(dbContext, _httpContextAccessor);

            // Act
            var userClubs = await dashboardRepository.GetAllUserClubs();

            // Assert
            userClubs.Should().HaveCount(1);
            userClubs.First().Title.Should().Be("Club 1");
        }

        [Fact]
        public async Task GetAllUserRaces_ReturnsUserRaces()
        {
            // Arrange
            var dbContext = await GetDbContext();

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, "user1")
            }));

            A.CallTo(() => _httpContextAccessor.HttpContext.User).Returns(claimsPrincipal);

            var dashboardRepository = new DashboardRepository(dbContext, _httpContextAccessor);

            // Act
            var userRaces = await dashboardRepository.GetAllUserRaces();

            // Assert
            userRaces.Should().HaveCount(1);
            userRaces.First().Title.Should().Be("Race 1");
        }
    }
}
