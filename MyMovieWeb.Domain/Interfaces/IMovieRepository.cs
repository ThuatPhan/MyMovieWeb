using MyMovieWeb.Domain.Entities;

namespace MyMovieWeb.Domain.Interfaces
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task<Movie?> GetByIdIncludeGenresAsync(int id);
        Task<IEnumerable<Movie>> GetAllIncludeGenresAsync();
        Task<int> GetTotalMovieCountAsync();
        Task<IEnumerable<Movie>> GetPagedMoviesAsync(int pageNumber, int pageSize);
    }
}
