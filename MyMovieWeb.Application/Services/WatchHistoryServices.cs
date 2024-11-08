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
        private readonly IMovieRepository _movieRepo;
        private readonly IEpisodeRepository _episodeRepo;
        private readonly IWatchHistoryRepository _watchHistoryRepo;

        public WatchHistoryServices(IMapper mapper, IMovieRepository movieRepo, IEpisodeRepository episodeRepo, IWatchHistoryRepository watchHistoryRepo)
        {
            _mapper = mapper;
            _movieRepo = movieRepo;
            _episodeRepo = episodeRepo;
            _watchHistoryRepo = watchHistoryRepo;
        }

        public async Task<Result<WatchHistoryDTO>> CreateWatchMovieLog(WatchMovieRequestDTO watchMovieRequest, string userId)
        {
            WatchHistory watchHistory = _mapper.Map<WatchHistory>(watchMovieRequest);
            watchHistory.UserId = userId;

            Movie? watchingMovie = await _movieRepo.GetByIdAsync(watchMovieRequest.MovieId);

            if (watchingMovie is null)
            {
                return Result<WatchHistoryDTO>.Failure($"Movie id {watchMovieRequest.MovieId} not found");
            }

            await _watchHistoryRepo.AddAsync(watchHistory);

            WatchHistoryDTO watchHistoryDTO = _mapper.Map<WatchHistoryDTO>(watchHistory);

            return Result<WatchHistoryDTO>.Success(watchHistoryDTO, "Watch movie log created successfully");
        }

        public async Task<Result<WatchHistoryDTO>> CreateWatchEpisodeLog(WatchEpisodeRequestDTO watchEpisodeRequest, string userId)
        {
            WatchHistory watchHistory = _mapper.Map<WatchHistory>(watchEpisodeRequest);
            watchHistory.UserId = userId;

            Movie? watchingMovie = await _movieRepo.GetByIdAsync(watchEpisodeRequest.MovieId);
            if (watchingMovie is null)
            {
                return Result<WatchHistoryDTO>.Failure($"Movie id {watchEpisodeRequest.MovieId} not found");
            }

            Episode? watchingEpisode = await _episodeRepo.GetByIdAsync(watchEpisodeRequest.EpisodeId);
            if (watchingEpisode is null)
            {
                return Result<WatchHistoryDTO>.Failure($"Episode id {watchEpisodeRequest.EpisodeId} not found");
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

            IEnumerable<WatchHistory> watchHistories = await _watchHistoryRepo.GetUserWatchHistoryAsync(guestId);

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
            WatchHistory? watchHistory = await _watchHistoryRepo.GetCurrentWatchingTimeAsync(userId, movieId, episodeId);
            if (watchHistory is null)
            {
                return Result<WatchHistoryDTO>.Failure("No watch history found for this movie or episode");
            }
            WatchHistoryDTO watchHistoryDTO = _mapper.Map<WatchHistoryDTO>(watchHistory);

            return Result<WatchHistoryDTO>.Success(watchHistoryDTO, "Current watching time retrieved successfully");
        }

        public async Task<Result<List<WatchHistoryDTO>>> GetWatchHistory(string userId)
        {
            IEnumerable<WatchHistory> watchHistories = await _watchHistoryRepo.GetUserWatchHistoryAsync(userId);

            if (userId.IsNullOrEmpty())
            {
                return Result<List<WatchHistoryDTO>>.Failure("User id cannot be empty");
            }

            List<WatchHistoryDTO> watchHistoryDTOs = _mapper.Map<List<WatchHistoryDTO>>(watchHistories);

            return Result<List<WatchHistoryDTO>>.Success(watchHistoryDTOs, "Watch histories retrieved successfully");

        }

    }
}
