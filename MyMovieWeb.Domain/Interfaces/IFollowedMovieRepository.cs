using MyMovieWeb.Domain.Entities;

namespace MyMovieWeb.Domain.Interfaces
{
    public interface IFollowedMovieRepository : IRepository<FollowedMovie>
    {
        Task<int> CountByUserIdAsync(string userId);
        Task<IEnumerable<FollowedMovie>> GetByUserIdIncludeMovie(string userId);
    }
}
