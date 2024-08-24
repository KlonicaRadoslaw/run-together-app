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
    public class RaceRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private ApplicationDbContext _dbContext;
        private RaceRepository _raceRepository;

        public RaceRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

            _dbContext = new ApplicationDbContext(_dbContextOptions);
            _raceRepository = new RaceRepository(_dbContext);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
        }

        

        [Fact]
        public async Task GetCountAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            _dbContext.Races.Add(new Race { Id = 1, Title = "Race 1", Description = "Desc 1" });
            _dbContext.Races.Add(new Race { Id = 2, Title = "Race 2", Description = "Desc 2" });
            _dbContext.SaveChanges();

            // Act
            var result = await _raceRepository.GetCountAsync();

            // Assert
            result.Should().Be(2);
        }

        [Fact]
        public async Task Add_ShouldAddRaceToDatabase()
        {
            // Arrange
            var race = new Race { Id = 1, Title = "Race 1", Description = "Desc 1" };

            // Act
            var result = _raceRepository.Add(race);

            // Assert
            result.Should().BeTrue();
            _dbContext.Races.Should().HaveCount(1);
        }

        [Fact]
        public async Task Delete_ShouldRemoveRaceFromDatabase()
        {
            // Arrange
            var race = new Race { Id = 1, Title = "Race 1", Description = "Desc 1" };
            _dbContext.Races.Add(race);
            _dbContext.SaveChanges();

            // Act
            var result = _raceRepository.Delete(race);

            // Assert
            result.Should().BeTrue();
            _dbContext.Races.Should().BeEmpty();
        }
    }
}
