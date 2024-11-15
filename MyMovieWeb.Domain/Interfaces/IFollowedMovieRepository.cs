using MyMovieWeb.Domain.Entities;
using System.Linq.Expressions;

namespace MyMovieWeb.Domain.Interfaces
{
    public interface IFollowedMovieRepository : IRepository<FollowedMovie>
    {
        Task<IEnumerable<FollowedMovie>> FindAllAsync(
            int pageNumber, int pageSize,
            Expression<Func<FollowedMovie, bool>> predicate,
            Func<IQueryable<FollowedMovie>, IOrderedQueryable<FollowedMovie>>? orderBy = null
        );

    }
}
