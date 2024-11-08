using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;
using MyMovieWeb.Infrastructure.Data;

namespace MyMovieWeb.Infrastructure.Repositories
{
    public class FollowedMovieRepository : Repository<FollowedMovie>, IFollowedMovieRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public FollowedMovieRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CountByUserIdAsync(string userId)
        {
            return await _dbContext.FollowedMovies
                .Where(fm => fm.UserId == userId)
                .CountAsync();
        }

        public async Task<IEnumerable<FollowedMovie>> GetByUserIdIncludeMovie(string userId)
        {
            return await _dbContext.FollowedMovies
                .Where(fm => fm.UserId == userId)
                .Include(fm => fm.Movie)
                    .ThenInclude(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Include(fm => fm.Movie)
                    .ThenInclude(m => m.Episodes)
                .ToListAsync();
        }
    }
}
