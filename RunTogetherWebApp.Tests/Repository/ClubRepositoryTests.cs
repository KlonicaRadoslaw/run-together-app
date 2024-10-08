﻿using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RunTogetherWebApp.Data;
using RunTogetherWebApp.Data.Enum;
using RunTogetherWebApp.Interfaces;
using RunTogetherWebApp.Models;
using RunTogetherWebApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTogetherWebApp.Tests.Repository
{
    public class ClubRepositoryTests
    {
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            if (await databaseContext.Clubs.CountAsync() == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    databaseContext.Clubs.Add(
                      new Club()
                      {
                          Title = $"Running Club {i + 1}",
                          Image = "https://www.eatthis.com/wp-content/uploads/sites/4/2020/05/running.jpg?quality=82&strip=1&resize=640%2C360",
                          Description = "This is the description of the first cinema",
                          ClubCategory = ClubCategory.City,
                          Address = new Address()
                          {
                              Street = "123 Main St",
                              City = "Charlotte",
                              State = "NC"
                          }
                      });
                }
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        [Fact]
        public async void ClubRepository_Add_ReturnsBool()
        {
            //Arrange
            var club = new Club()
            {
                Title = "Running Club 1",
                Image = "https://www.eatthis.com/wp-content/uploads/sites/4/2020/05/running.jpg?quality=82&strip=1&resize=640%2C360",
                Description = "This is the description of the first cinema",
                ClubCategory = ClubCategory.City,
                Address = new Address()
                {
                    Street = "123 Main St",
                    City = "Charlotte",
                    State = "NC"
                }
            };

            var dbContext = await GetDbContext();
            var clubRepository = new ClubRepository(dbContext);

            //Act
            var result = clubRepository.Add(club);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void ClubRepository_GetById_ReturnsClub()
        {
            //Arrange
            var id = 1;
            var dbContext = await GetDbContext();
            var clubRepository = new ClubRepository(dbContext);

            //Act
            var result = clubRepository.GetById(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<Club>>();
        }

        [Fact]
        public async Task Delete_DeletesClubFromDatabase()
        {
            // Arrange
            var club = new Club()
            {
                Title = "Running Club 111",
                Image = "https://www.eatthis.com/wp-content/uploads/sites/4/2020/05/running.jpg?quality=82&strip=1&resize=640%2C360",
                Description = "This is the description of the first cinema",
                ClubCategory = ClubCategory.City,
                Address = new Address()
                {
                    Street = "123 Main St",
                    City = "Charlotte",
                    State = "NC"
                }
            };

            var dbContext = await GetDbContext();
            var clubRepository = new ClubRepository(dbContext);

            dbContext.Clubs.Add(club);
            await dbContext.SaveChangesAsync();

            // Act
            var result = clubRepository.Delete(club);
            var clubs = await dbContext.Clubs.ToListAsync();

            // Assert
            result.Should().BeTrue();
            clubs.Should().NotContain(c => c.Title == "Running Club 111");
        }

        [Fact]
        public async void GetAll_ReturnsAllClubs()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var clubRepository = new ClubRepository(dbContext);

            // Act
            var result = await clubRepository.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(10);
        }

        [Fact]
        public async Task GetClubByCity_ReturnsClubsInCity()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var clubRepository = new ClubRepository(dbContext);


            // Act
            var result = await clubRepository.GetClubByCity("Charlotte");

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.First().Title.Should().Be("Running Club 1");
        }
    }
}
