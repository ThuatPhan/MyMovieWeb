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

        public async Task<IEnumerable<WatchHistory>> GetUserWatchHistoryAsync(string userId)
        {
            return await _dbContext.Set<WatchHistory>()
                .Where(wh => wh.UserId == userId)
                .Distinct()
                .ToListAsync();
        }
    }
}
