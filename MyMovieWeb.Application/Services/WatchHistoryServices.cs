using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;

namespace MyMovieWeb.Application.Services
{
    public class WatchHistoryServices : IWatchHistoryServices
    {
        private readonly IMapper _mapper;
        private readonly IMovieService _movieServices;
        private readonly IEpisodeServices _episodeServices;
        private readonly IWatchHistoryRepository _watchHistoryRepo;

        public WatchHistoryServices(IMapper mapper, IMovieService movieService, IEpisodeServices episodeServices, IWatchHistoryRepository watchHistoryRepo)
        {
            _mapper = mapper;
            _movieServices = movieService;
            _episodeServices = episodeServices;
            _watchHistoryRepo = watchHistoryRepo;
        }

        public async Task<Result<WatchHistoryDTO>> CreateWatchMovieLog(WatchMovieRequestDTO watchMovieRequest, string userId)
        {
            WatchHistory watchHistory = _mapper.Map<WatchHistory>(watchMovieRequest);
            watchHistory.UserId = userId;

            Result<MovieDTO> result = await _movieServices.GetMovieById(watchMovieRequest.MovieId);
            if (!result.IsSuccess)
            {
                return Result<WatchHistoryDTO>.Failure(result.Message);
            }

            await _watchHistoryRepo.AddAsync(watchHistory);

            WatchHistoryDTO watchHistoryDTO = _mapper.Map<WatchHistoryDTO>(watchHistory);

            return Result<WatchHistoryDTO>.Success(watchHistoryDTO, "Watch movie log created successfully");
        }

        public async Task<Result<WatchHistoryDTO>> CreateWatchEpisodeLog(WatchEpisodeRequestDTO watchEpisodeRequest, string userId)
        {
            WatchHistory watchHistory = _mapper.Map<WatchHistory>(watchEpisodeRequest);
            watchHistory.UserId = userId;

            Result<MovieDTO> movieResult = await _movieServices.GetMovieById(watchEpisodeRequest.MovieId);
            if (!movieResult.IsSuccess)
            {
                return Result<WatchHistoryDTO>.Failure(movieResult.Message);
            }

            Result<EpisodeDTO> episodeResult = await _episodeServices.GetEpisodeById(watchEpisodeRequest.EpisodeId);
            if (!episodeResult.IsSuccess)
            {
                return Result<WatchHistoryDTO>.Failure(episodeResult.Message);
            }

            await _watchHistoryRepo.AddAsync(watchHistory);

            WatchHistoryDTO watchHistoryDTO = _mapper.Map<WatchHistoryDTO>(watchHistory);

            return Result<WatchHistoryDTO>.Success(watchHistoryDTO, "Watch episode log created successfully");
        }

        public async Task<Result<List<WatchHistoryDTO>>> UpdateGuestToUserWatchHistory(string guestId, string userId)
        {
            if (guestId.IsNullOrEmpty())
            {
                return Result<List<WatchHistoryDTO>>.Failure("Guest id cannot be empty");
            }
            if (userId.IsNullOrEmpty())
            {
                return Result<List<WatchHistoryDTO>>.Failure("User id cannot be empty");
            }

            IEnumerable<WatchHistory> watchHistories = await _watchHistoryRepo.FindAllAsync(wh => wh.UserId == userId);

            if (!watchHistories.Any())
            {
                return Result<List<WatchHistoryDTO>>.Failure($"No histories of guest id {guestId} was found");
            }

            foreach (var log in watchHistories)
            {
                log.UserId = userId;
            }

            await _watchHistoryRepo.UpdateRangeAsync(watchHistories);

            List<WatchHistoryDTO> watchHistoryDTOs = _mapper.Map<List<WatchHistoryDTO>>(watchHistories);

            return Result<List<WatchHistoryDTO>>.Success(watchHistoryDTOs, "Watch histories updated successfully");

        }

        public async Task<Result<WatchHistoryDTO>> GetCurrentWatchingTime(string userId, int movieId, int? episodeId = null)
        {
            if (userId.IsNullOrEmpty())
            {
                return Result<WatchHistoryDTO>.Failure("User id cannot be empty");
            }

            WatchHistory? watchHistory = await _watchHistoryRepo.GetCurrentWatchingTimeAsync(userId, movieId, episodeId);

            WatchHistoryDTO watchHistoryDTO = _mapper.Map<WatchHistoryDTO>(watchHistory);

            return Result<WatchHistoryDTO>.Success(watchHistoryDTO, "Current watching time retrieved successfully");
        }

        public async Task<Result<int>> CountWatchHistories(string userId)
        {
            int totalCount = await _watchHistoryRepo.CountAsync(wh => wh.UserId == userId);
            return Result<int>.Success(totalCount, "Watch histories counted successfully");
        }

        public async Task<Result<WatchHistoryDTO>> GetWatchHistory(int id, string userId)
        {
            WatchHistory? watchHistory = await _watchHistoryRepo.GetWatchHistoryAsync(id, userId);
            if (watchHistory is null)
            {
                return Result<WatchHistoryDTO>.Failure($"Watch history id {id} of user id {userId} not found");
            }

            WatchHistoryDTO watchHistoryDTO = _mapper.Map<WatchHistoryDTO>(watchHistory);

            if (watchHistory.Movie.IsSeries && watchHistory.EpisodeId.HasValue)
            {
                Result<EpisodeDTO> result = await _episodeServices.GetEpisodeById(watchHistory.EpisodeId.Value);
                if (!result.IsSuccess)
                {
                    return Result<WatchHistoryDTO>.Failure($"Failed when retrieving current watching episode ${result.Message}");
                }
                watchHistoryDTO.CurrentEpisode = result.Data;
            }

            return Result<WatchHistoryDTO>.Success(watchHistoryDTO, "Watch history retrieved sucessfully");
        }

        public async Task<Result<List<WatchHistoryDTO>>> GetWatchHistories(int pageNumber, int pageSize, string userId)
        {
            IEnumerable<WatchHistory> watchHistories = await _watchHistoryRepo.GetWatchHistoriesAsync(pageNumber, pageSize, userId);

            if (userId.IsNullOrEmpty())
            {
                return Result<List<WatchHistoryDTO>>.Failure("User id cannot be empty");
            }

            List<WatchHistoryDTO> watchHistoryDTOs = _mapper.Map<List<WatchHistoryDTO>>(watchHistories);

            return Result<List<WatchHistoryDTO>>.Success(watchHistoryDTOs, "Watch histories retrieved successfully");

        }

    }
}
