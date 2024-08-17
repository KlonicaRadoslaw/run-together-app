using RunTogetherWebApp.Data.Enum;
using RunTogetherWebApp.Models;

namespace RunTogetherWebApp.Interfaces
{
    public interface IClubRepository
    {
        Task<IEnumerable<Club>> GetAll();
        Task<Club> GetById(int id);
        Task<Club> GetByIdNoTracking(int id);
        Task<IEnumerable<Club>> GetClubByCity(string city);
        Task<IEnumerable<Club>> GetSliceAsync(int offset, int size);
        Task<int> GetCountAsync();
        Task<IEnumerable<Club>> GetClubsByCategoryAndSliceAsync(ClubCategory category, int offset, int size);
        Task<int> GetCountByCategoryAsync(ClubCategory category);
        bool Add(Club club);
        bool Update(Club club);
        bool Delete(Club club);
        bool Save();
    }
}
