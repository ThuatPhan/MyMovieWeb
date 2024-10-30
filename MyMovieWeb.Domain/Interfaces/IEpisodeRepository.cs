using MyMovieWeb.Domain.Entities;

namespace MyMovieWeb.Domain.Interfaces
{
    public interface IEpisodeRepository : IRepository<Episode>
    {
        Task<int> GetTotalEpisodeCountAsync(int movieId);
        Task<IEnumerable<Episode>> GetEpisodesAsync(int movieId);
        Task<IEnumerable<Episode>> GetPagedEpisodesAsync(int movieId, int pageNumber, int pageSize);
    }
}
