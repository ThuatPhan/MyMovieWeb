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
        public async Task<IEnumerable<Episode>> GetEpisodeByMovieIdAsync(int movieId)
        {
            return await _dbContext.Episodes.Where(e => e.MovieId == movieId).ToListAsync();
        }
    }
}
