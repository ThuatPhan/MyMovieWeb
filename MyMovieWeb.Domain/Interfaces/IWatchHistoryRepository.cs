using MyMovieWeb.Domain.Entities;

namespace MyMovieWeb.Domain.Interfaces
{
    public interface IWatchHistoryRepository : IRepository<WatchHistory>
    {
        Task<IEnumerable<WatchHistory>> GetUserWatchHistoryAsync(string userId);
    }
}
