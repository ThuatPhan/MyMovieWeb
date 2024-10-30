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

        public async Task<Result<bool>> CreateWatchMovieLog(WatchMovieRequestDTO watchMovieRequest, string userId)
        {
            WatchHistory watchHistory = _mapper.Map<WatchHistory>(watchMovieRequest);
            watchHistory.UserId = userId;

            Movie? watchingMovie = await _movieRepo.GetByIdAsync(watchMovieRequest.MovieId);

            if (watchingMovie is null)
            {
                return Result<bool>.Failure($"Movie id {watchMovieRequest.MovieId} not found");
            }

            await _watchHistoryRepo.AddAsync(watchHistory);

            return Result<bool>.Success(true, "Watch movie log created successfully");
        }

        public async Task<Result<bool>> CreateWatchEpisodeLog(WatchEpisodeRequestDTO watchEpisodeRequest, string userId)
        {
            WatchHistory watchHistory = _mapper.Map<WatchHistory>(watchEpisodeRequest);
            watchHistory.UserId = userId;

            Movie? watchingMovie = await _movieRepo.GetByIdAsync(watchEpisodeRequest.MovieId);
            if (watchingMovie is null)
            {
                return Result<bool>.Failure($"Movie id {watchEpisodeRequest.MovieId} not found");
            }

            Episode? watchingEpisode = await _episodeRepo.GetByIdAsync(watchEpisodeRequest.EpisodeId);
            if (watchingEpisode is null)
            {
                return Result<bool>.Failure($"Episode id {watchEpisodeRequest.EpisodeId} not found");
            }

            await _watchHistoryRepo.AddAsync(watchHistory);

            return Result<bool>.Success(true, "Watch episode log created successfully");
        }

        public async Task<Result<List<MovieDTO>>> GetUserWatchHistory(string userId)
        {
            IEnumerable<WatchHistory> watchHistories = await _watchHistoryRepo.GetUserWatchHistoryAsync(userId);

            if (userId.IsNullOrEmpty())
            {
                return Result<List<MovieDTO>>.Failure("User id cannot be empty");
            }

            List<int> genreIds = watchHistories
                .Select(wh => wh.MovieId)
                .ToList();

            IEnumerable<Movie> movies = await _movieRepo.GetByIdsIncludeGenresAsync(genreIds);

            List<MovieDTO> movieDTOs = _mapper.Map<List<MovieDTO>>(movies);

            return Result<List<MovieDTO>>.Success(movieDTOs, "User watch history retrieved successfully");
        }

    }
}
