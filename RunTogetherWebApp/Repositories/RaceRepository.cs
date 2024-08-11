﻿using Microsoft.EntityFrameworkCore;
using RunTogetherWebApp.Data;
using RunTogetherWebApp.Interfaces;
using RunTogetherWebApp.Models;

namespace RunTogetherWebApp.Repositories
{
    public class RaceRepository : IRaceRepository
    {
        private readonly ApplicationDbContext _context;
        public RaceRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool Add(Race race)
        {
            _context.Add(race);
            return Save();
        }

        public bool Delete(Race race)
        {
            _context.Remove(race);
            return Save();
        }

        public async Task<IEnumerable<Race>> GetAll()
        {
            return await _context.Races.ToListAsync();
        }

        public async Task<IEnumerable<Race>> GetAllByCity(string city)
        {
            return await _context.Races
                                    .Where(r => r.Address.City.Contains(city))
                                    .ToListAsync();
        }

        public async Task<Race> GetById(int id)
        {
            return await _context.Races
                .Include(a => a.Address)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Race> GetByIdNoTracking(int id)
        {
            return await _context.Races
                .Include(c => c.Address)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Race race)
        {
            _context.Update(race);
            return Save();
        }
    }
}
