using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;
using MyMovieWeb.Infrastructure.Data;

namespace MyMovieWeb.Infrastructure.Repositories
{
    public class EpisodeRepository : Repository<Episode>, IEpisodeRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EpisodeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> GetTotalEpisodeCountAsync(int movieId)
        {
            return await _dbContext.Episodes
                .Where(e => e.MovieId == movieId)
                .CountAsync();
        }

        public async Task<IEnumerable<Episode>> GetPagedEpisodesAsync(int movieId, int pageNumber, int pageSize)
        {
            return await _dbContext.Episodes
                .Where(e => e.MovieId == movieId)
                .Skip((pageSize - 1) * pageNumber)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Episode>> GetEpisodesAsync(int movieId)
        {
            return await _dbContext.Episodes
                .Where(e => e.MovieId == movieId)
                .ToListAsync();
        }

    }
}
