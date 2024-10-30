using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;
using MyMovieWeb.Infrastructure.Data;

namespace MyMovieWeb.Infrastructure.Repositories
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public MovieRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Movie>> GetAllIncludeGenresAsync()
        {
            return await _dbContext.Movies
                 .Include(m => m.MovieGenres)
                 .ThenInclude(mg => mg.Genre)
                 .Include(m => m.Episodes)
                 .ToListAsync();
        }

        public async Task<Movie?> GetByIdIncludeGenresAsync(int id)
        {
            return await _dbContext.Movies
                .Where(m => m.Id == id)
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .Include(m => m.Episodes)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Movie>> GetPagedMoviesAsync(int pageNumber, int pageSize)
        {
            return await _dbContext.Movies
                 .Include(m => m.MovieGenres)
                 .ThenInclude(mg => mg.Genre)
                 .Include(m => m.Episodes)
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
        }

        public async Task<int> CountByGenreAsync(int genreId)
        {
            return await _dbContext.Movies
               .Include(m => m.MovieGenres)
               .ThenInclude(mg => mg.Genre)
               .Where(m => m.MovieGenres.Any(mg => mg.GenreId == genreId))
               .CountAsync();
        }

        public async Task<IEnumerable<Movie>> GetPagedMoviesByGenreAsync(int genreId, int pageNumber, int pageSize)
        {
            return await _dbContext.Movies
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .Where(m => m.MovieGenres.Any(mg => mg.GenreId == genreId))
                .Include(m => m.Episodes)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetByIdsIncludeGenresAsync(List<int> movieIds)
        {
            return await _dbContext.Set<Movie>()
                .Where(m => movieIds.Contains(m.Id))
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .Include(m => m.Episodes)
                .ToListAsync();
        }
    }
}
