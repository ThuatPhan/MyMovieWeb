using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IRepository<WatchHistory> _watchHistoryRepo;
        private readonly IRepository<Comment> _commentRepo;

        public WatchHistoryServices(
            IMapper mapper,
            IMovieService movieService,
            IEpisodeServices episodeServices,
            IRepository<WatchHistory> watchHistoryRepository,
            IRepository<Comment> commentRepository
,
            IMemoryCache memoryCache)
        {
            _mapper = mapper;
            _movieServices = movieService;
            _episodeServices = episodeServices;
            _watchHistoryRepo = watchHistoryRepository;
            _commentRepo = commentRepository;
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

            var watched = await _watchHistoryRepo.
                GetBaseQuery(wh => wh.UserId == userId && wh.MovieId == watchMovieRequest.MovieId && !wh.IsWatched)
                .OrderByDescending(wh => wh.LogDate)
                .FirstOrDefaultAsync();

            if (watched is null)
            {
                await _movieServices.IncreaseView(result.Data.Id, 1);
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


            var watched = await _watchHistoryRepo.
                GetBaseQuery(
                    wh => wh.UserId == userId
                    && wh.MovieId == watchEpisodeRequest.MovieId
                    && wh.EpisodeId == watchEpisodeRequest.EpisodeId
                    && !wh.IsWatched
                )
                .OrderByDescending(wh => wh.LogDate)
                .FirstOrDefaultAsync();

            if (watched is null)
            {
                await _movieServices.IncreaseView(movieResult.Data.Id, 1);
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

        public async Task<Result<bool>> DeleteWatchHistoryOfMovie(int movieId)
        {
            await _watchHistoryRepo.RemoveRangeAsync(wh => wh.MovieId == movieId);
            return Result<bool>.Success(true, $"Watch histories of movie id {movieId} deleted successfully");
        }

        public async Task<Result<bool>> MarkWatchHistoryWatched(string userId, int movieId)
        {
            if (userId.IsNullOrEmpty())
            {
                return Result<bool>.Failure("User id can't be empty");
            }

            IEnumerable<WatchHistory> watchHistories = await _watchHistoryRepo
                .FindAllAsync(wh => wh.UserId == userId && wh.MovieId == movieId);

            if (!watchHistories.Any())
            {
                return Result<bool>.Failure("No watch histories found");
            }

            foreach (var watchHistory in watchHistories)
            {
                watchHistory.IsWatched = true;
            }

            await _watchHistoryRepo.UpdateRangeAsync(watchHistories);
            return Result<bool>.Success(true, "Mark histories watched successfully");
        }

        public async Task<Result<WatchHistoryDTO>> GetCurrentWatchingTime(string userId, int movieId, int? episodeId = null)
        {
            if (userId.IsNullOrEmpty())
            {
                return Result<WatchHistoryDTO>.Failure("User id can't be empty");
            }

            IQueryable<WatchHistory> query = _watchHistoryRepo
                .GetBaseQuery(predicate: wh => wh.UserId == userId && wh.MovieId == movieId && wh.EpisodeId == episodeId && !wh.IsWatched);

            WatchHistory? watchHistory = await query
                .Include(wh => wh.Movie)
                    .ThenInclude(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .OrderByDescending(wh => wh.CurrentWatching)
                .FirstOrDefaultAsync();

            WatchHistoryDTO watchHistoryDTO = _mapper.Map<WatchHistoryDTO>(watchHistory);

            return Result<WatchHistoryDTO>.Success(watchHistoryDTO, "Current watching time retrieved successfully");
        }

        public async Task<Result<int>> CountWatchHistories(string userId)
        {
            IQueryable<WatchHistory> query = _watchHistoryRepo
                .GetBaseQuery(wh => wh.UserId == userId)
                .GroupBy(wh => wh.MovieId)
                .Select(wh => wh
                    .OrderByDescending(wh => wh.LogDate)
                    .First()
                );

            int totalCount = await query.CountAsync();
            return Result<int>.Success(totalCount, "Watch histories counted successfully");
        }

        public async Task<Result<int>> CountWatchingHistories(string userId)
        {
            IQueryable<WatchHistory> query = _watchHistoryRepo
                .GetBaseQuery(wh => wh.UserId == userId && !wh.IsWatched)
                .GroupBy(wh => wh.MovieId)
                .Select(wh => wh
                    .OrderByDescending(wh => wh.LogDate)
                    .First()
                );

            int totalCount = await query.CountAsync();
            return Result<int>.Success(totalCount, "Watch histories counted successfully");
        }

        public async Task<Result<WatchHistoryDTO>> GetWatchHistory(int id, string userId)
        {
            IQueryable<WatchHistory> query = _watchHistoryRepo
                .GetBaseQuery(predicate: wh => wh.Id == id && wh.UserId == userId && !wh.IsWatched)
                .Include(wh => wh.Movie)
                    .ThenInclude(m => m.MovieGenres)
                        .ThenInclude(mg => mg.Genre)
                .Include(wh => wh.Movie)
                    .ThenInclude(m => m.Episodes);

            WatchHistory? watchHistory = await query.FirstOrDefaultAsync();

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
            if (userId.IsNullOrEmpty())
            {
                return Result<List<WatchHistoryDTO>>.Failure("User id cannot be empty");
            }

            List<WatchHistory> watchHistories = await _watchHistoryRepo
                .GetBaseQuery(wh => wh.UserId == userId)
                .Include(wh => wh.Movie)
                    .ThenInclude(m => m.MovieGenres)
                        .ThenInclude(mg => mg.Genre)
                .GroupBy(wh => wh.MovieId)
                .Select(group => group.OrderByDescending(group => group.LogDate).First())
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            List<WatchHistoryDTO> watchHistoryDTOs = _mapper.Map<List<WatchHistoryDTO>>(watchHistories);

            foreach (var watchHistoryDTO in watchHistoryDTOs)
            {
                watchHistoryDTO.Movie.CommentCount = await _commentRepo.CountAsync(c => c.MovieId == watchHistoryDTO.Movie.Id);
            }

            return Result<List<WatchHistoryDTO>>.Success(watchHistoryDTOs, "Watch histories retrieved successfully");
        }

        public async Task<Result<List<WatchHistoryDTO>>> GetWatchingHistories(int pageNumber, int pageSize, string userId)
        {
            if (userId.IsNullOrEmpty())
            {
                return Result<List<WatchHistoryDTO>>.Failure("User id cannot be empty");
            }

            List<WatchHistory> watchHistories = await _watchHistoryRepo
                .GetBaseQuery(wh => wh.UserId == userId && !wh.IsWatched)
                .Include(wh => wh.Movie)
                    .ThenInclude(m => m.MovieGenres)
                        .ThenInclude(mg => mg.Genre)
                .GroupBy(wh => wh.MovieId)
                .Select(group => group.OrderByDescending(group => group.LogDate).First())
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            List<WatchHistoryDTO> watchHistoryDTOs = _mapper.Map<List<WatchHistoryDTO>>(watchHistories);

            foreach (var watchHistoryDTO in watchHistoryDTOs)
            {
                watchHistoryDTO.Movie.CommentCount = await _commentRepo.CountAsync(c => c.MovieId == watchHistoryDTO.Movie.Id);
            }

            return Result<List<WatchHistoryDTO>>.Success(watchHistoryDTOs, "Watch histories retrieved successfully");
        }

    }
}
