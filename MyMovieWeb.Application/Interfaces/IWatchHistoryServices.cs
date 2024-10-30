using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IWatchHistoryServices
    {
        Task<Result<bool>> CreateWatchMovieLog(WatchMovieRequestDTO watchMovieRequest, string userId);
        Task<Result<bool>> CreateWatchEpisodeLog(WatchEpisodeRequestDTO watchEpisodeRequest, string userId);
        Task<Result<List<MovieDTO>>> GetUserWatchHistory(string userId);
    }
}
