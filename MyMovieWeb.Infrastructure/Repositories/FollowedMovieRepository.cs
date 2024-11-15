using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;
using MyMovieWeb.Infrastructure.Data;
using System.Linq.Expressions;

namespace MyMovieWeb.Infrastructure.Repositories
{
    public class FollowedMovieRepository : Repository<FollowedMovie>, IFollowedMovieRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public FollowedMovieRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<FollowedMovie>> FindAllAsync(
            int pageNumber, 
            int pageSize,
            Expression<Func<FollowedMovie, bool>> predicate, 
            Func<IQueryable<FollowedMovie>, IOrderedQueryable<FollowedMovie>>? orderBy = null
        )
        {
            IQueryable<FollowedMovie> query = _dbContext.FollowedMovies
                .Where(predicate)
                .Include(fm => fm.Movie)
                    .ThenInclude(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre);

            if(orderBy != null)
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
