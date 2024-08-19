using RunTogetherWebApp.Data.Enum;
using RunTogetherWebApp.Models;

namespace RunTogetherWebApp.Interfaces
{
    public interface IRaceRepository
    {
        Task<IEnumerable<Race>> GetAll();
        Task<Race> GetById(int id);
        Task<Race> GetByIdNoTracking(int id);
        Task<IEnumerable<Race>> GetAllByCity(string city);
        Task<int> GetCountAsync();
        Task<int> GetCountByCategoryAsync(RaceCategory category);
        Task<IEnumerable<Race>> GetSliceAsync(int offset, int size);
        Task<IEnumerable<Race>> GetRacesByCategoryAndSliceAsync(RaceCategory category, int offset, int size);
        bool Add(Race race);
        bool Update(Race race);
        bool Delete(Race race);
        bool Save();
    }
}
