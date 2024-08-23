using RunTogetherWebApp.Models;

namespace RunTogetherWebApp.Interfaces
{
    public interface ILocationService
    {
        Task<List<City>> GetLocationSearch(string location);
    }
}
