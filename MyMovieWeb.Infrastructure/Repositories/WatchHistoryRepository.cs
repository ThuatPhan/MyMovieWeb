using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;
using MyMovieWeb.Infrastructure.Data;

namespace MyMovieWeb.Infrastructure.Repositories
{
    public class WatchHistoryRepository : Repository<WatchHistory>, IWatchHistoryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public WatchHistoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<WatchHistory?> GetCurrentWatchingTimeAsync(string userId, int movieId, int? episodeId = null)
        {
            return await _dbContext.Set<WatchHistory>()
                 .Where(wh => wh.UserId == userId && wh.MovieId == movieId && wh.EpisodeId == episodeId)
                 .GroupBy(wh => wh.MovieId)
                 .Select(wh => wh.OrderByDescending(wh => wh.CurrentWatching).First())
                 .FirstOrDefaultAsync();
        }

        public async Task<WatchHistory?> GetWatchHistoryAsync(int id, string userId)
        {
            return await _dbContext.Set<WatchHistory>()
                .Where(wh => wh.Id == id && wh.UserId == userId)
                .Include(wh => wh.Movie)
                    .ThenInclude(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Include(wh => wh.Movie)
                    .ThenInclude(m => m.Episodes)
                .GroupBy(wh => wh.MovieId)
                .Select(wh => wh.OrderByDescending(wh => wh.LogDate).First())
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<WatchHistory>> GetWatchHistoriesAsync(int pageNumber, int pageSize, string userId)
        {
            return await _dbContext.Set<WatchHistory>()
                .Where(wh => wh.UserId == userId)
                .Include(wh => wh.Movie)
                    .ThenInclude(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .GroupBy(wh => wh.MovieId)
                .Select(wh => wh.OrderByDescending(wh => wh.LogDate).First())
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
