using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;
using MyMovieWeb.Infrastructure.Data;
using System.Linq.Expressions;

namespace MyMovieWeb.Infrastructure.Repositories
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public MovieRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Movie?> GetByIdIncludeGenresAsync(int id)
        {
            return await _dbContext.Movies
                .Where(m => m.Id == id)
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Movie>> GetAllIncludeGenresAsync()
        {
            return await _dbContext.Movies
                 .Include(m => m.MovieGenres)
                 .ThenInclude(mg => mg.Genre)
                 .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> FindAllIncludeGenresAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<Movie, bool>> predicate,
            Func<IQueryable<Movie>, IOrderedQueryable<Movie>>? orderBy = null
        )
        {
            IQueryable<Movie> query = _dbContext.Movies
                .Where(m => m.IsShow == true)
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .Where(predicate);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
