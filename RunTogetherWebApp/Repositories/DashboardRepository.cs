﻿using Microsoft.EntityFrameworkCore;
using RunTogetherWebApp.Data;
using RunTogetherWebApp.Extensions;
using RunTogetherWebApp.Interfaces;
using RunTogetherWebApp.Models;

namespace RunTogetherWebApp.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<Club>> GetAllUserClubs()
        {
            var currentUser = _httpContextAccessor.HttpContext?.User.GetUserId();
            var userClubs = await _context.Clubs
                                    .Where(r => r.AppUser.Id == currentUser)
                                    .ToListAsync();

            return userClubs;
        }

        public async Task<List<Race>> GetAllUserRaces()
        {
            var currentUser = _httpContextAccessor.HttpContext?.User.GetUserId();
            var userRaces = await _context.Races
                                    .Where(r => r.AppUser.Id == currentUser)
                                    .ToListAsync();

            return userRaces;
        }
    }
}
