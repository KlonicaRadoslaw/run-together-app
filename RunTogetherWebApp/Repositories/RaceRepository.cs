using Microsoft.EntityFrameworkCore;
using RunTogetherWebApp.Data;
using RunTogetherWebApp.Data.Enum;
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

        public async Task<int> GetCountAsync()
        {
            return await _context.Races.CountAsync();
        }

        public async Task<int> GetCountByCategoryAsync(RaceCategory category)
        {
            return await _context.Races.CountAsync(r => r.RaceCategory == category);
        }

        public async Task<IEnumerable<Race>> GetRacesByCategoryAndSliceAsync(RaceCategory category, int offset, int size)
        {
            return await _context.Races
                .Where(r => r.RaceCategory == category)
                .Include(a => a.Address)
                .Skip(offset)
                .Take(size)
                .ToListAsync();
        }

        public async Task<IEnumerable<Race>> GetSliceAsync(int offset, int size)
        {
            return await _context.Races
                .Skip(offset)
                .Take(size)
                .ToListAsync();
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
