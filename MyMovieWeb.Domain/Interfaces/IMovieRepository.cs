using MyMovieWeb.Domain.Entities;

namespace MyMovieWeb.Domain.Interfaces
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task<Movie?> GetByIdIncludeGenresAsync(int id);
        Task<IEnumerable<Movie>> GetByIdsIncludeGenresAsync(List<int> movieIds);
        Task<IEnumerable<Movie>> GetAllIncludeGenresAsync();
        Task<IEnumerable<Movie>> GetPagedMoviesAsync(int pageNumber, int pageSize);
        Task<int> CountByGenreAsync(int genreId);
        Task<IEnumerable<Movie>> GetPagedMoviesByGenreAsync(int genreId, int pageNumber, int pageSize);
    }
}
