﻿using MyMovieWeb.Domain.Entities;

namespace MyMovieWeb.Domain.Interfaces
{
    public interface IWatchHistoryRepository : IRepository<WatchHistory>
    {
        Task<WatchHistory?> GetCurrentWatchingTimeAsync(string userId, int movieId, int? episodeId = null);
        Task<WatchHistory?> GetWatchHistoryAsync(int id, string userId);
        Task<IEnumerable<WatchHistory>> GetWatchHistoriesAsync(int pageNumber, int pageSize, string userId);
    }
}
