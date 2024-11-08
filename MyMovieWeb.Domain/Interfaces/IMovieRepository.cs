using MyMovieWeb.Domain.Entities;
using System.Linq.Expressions;

namespace MyMovieWeb.Domain.Interfaces
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task<Movie?> GetByIdIncludeGenresAsync(int id);
        Task<IEnumerable<Movie>> GetAllIncludeGenresAsync();
        Task<IEnumerable<Movie>> GetPagedMoviesAsync(int pageNumber, int pageSize, bool? isShow = null);
        Task<int> CountByGenreAsync(int genreId);
        Task<IEnumerable<Movie>> GetPagedMoviesAsync(int pageNumber, int pageSize, Expression<Func<Movie, bool>> predicate);
        Task<IEnumerable<Movie>> GetPagedRecentAddedMoviesAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Movie>> FindAllAsync(int pageNumber, int pageSize, Expression<Func<Movie, bool>> predicate);
    }
}
