using MyMovieWeb.Domain.Entities;

namespace MyMovieWeb.Domain.Interfaces
{
    public interface IWatchHistoryRepository : IRepository<WatchHistory>
    {
        Task<WatchHistory?> GetCurrentWatchingTimeAsync(string userId, int movieId, int? episodeId = null);
        Task<IEnumerable<WatchHistory>> GetUserWatchHistoryAsync(string userId);
    }
}
