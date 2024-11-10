using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IWatchHistoryServices
    {
        Task<Result<WatchHistoryDTO>> CreateWatchMovieLog(WatchMovieRequestDTO watchMovieRequest, string userId);
        Task<Result<WatchHistoryDTO>> CreateWatchEpisodeLog(WatchEpisodeRequestDTO watchEpisodeRequest, string userId);
        Task<Result<List<WatchHistoryDTO>>> UpdateGuestToUserWatchHistory(string guestId, string userId);
        Task<Result<WatchHistoryDTO>> GetCurrentWatchingTime(string userId, int movieId, int? episodeId = null);
        Task<Result<int>> CountWatchHistories(string userId);
        Task<Result<WatchHistoryDTO>> GetWatchHistory(int id, string userId);
        Task<Result<List<WatchHistoryDTO>>> GetWatchHistories(int pageNumber, int pageSize, string userId);
    }
}
