using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;

namespace MyMovieWeb.Application.Services
{
    public class UserServices : IUserServices
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Comment> _commentRepo;
        private readonly IRepository<FollowedMovie> _followedMovieRepo;
        private readonly IMovieService _movieService;
        private readonly INotificationServices _notificationService;
        private readonly IRepository<Order> _orderRepo;

        public UserServices(
            IMapper mapper,
            IRepository<Comment> commentRepository,
            IRepository<FollowedMovie> followedMovieRepository,
            IMovieService movieService,
            INotificationServices notificationService,
            IRepository<Order> orderRepository
        )
        {
            _mapper = mapper;
            _movieService = movieService;
            _followedMovieRepo = followedMovieRepository;
            _commentRepo = commentRepository;
            _notificationService = notificationService;
            _orderRepo = orderRepository;
        }

        public async Task<Result<bool>> FollowMovie(int movieId, string userId)
        {

            Result<MovieDTO> result = await _movieService.GetMovieById(movieId);

            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.Message);
            }

            var existingMovie = await _followedMovieRepo.FindAllAsync(fm => fm.UserId == userId && fm.MovieId == movieId);
            if (existingMovie.Any())
            {
                return Result<bool>.Failure($"User id {userId} has followed movie id {movieId} before");
            }

            await _followedMovieRepo.AddAsync(new FollowedMovie { MovieId = result.Data.Id, UserId = userId });

            return Result<bool>.Success(true, "Movie followed successfully");
        }

        public async Task<Result<bool>> UnfollowMovie(int movieId, string userId)
        {
            FollowedMovie? followedMovie = await _followedMovieRepo.FindOneAsync(fm => fm.MovieId == movieId && fm.UserId == userId);
            if (followedMovie is null)
            {
                return Result<bool>.Failure($"User id {userId} has not followed movie id {movieId} before");
            }
            await _followedMovieRepo.RemoveAsync(followedMovie);
            return Result<bool>.Success(true, "Unfollow movie successfully");

        }

        public async Task<Result<bool>> IsUserFollowMovie(int movieId, string userId)
        {
            FollowedMovie? followedMovie = await _followedMovieRepo.FindOneAsync(fm => fm.MovieId == movieId && fm.UserId == userId);
            if (followedMovie is null)
            {
                return Result<bool>.Failure($"User id {userId} has not followed movie id {movieId} before");
            }

            return Result<bool>.Success(true, "User has followed movie before");
        }

        public async Task<Result<int>> CountFollowedMovie(string userId)
        {
            int totalCount = await _followedMovieRepo.CountAsync(fm => fm.UserId == userId);
            return Result<int>.Success(totalCount, "Followed movie count retrived successfully");
        }

        public async Task<Result<List<FollowedMovieDTO>>> GetFollowedMovies(string userId, int pageNumber, int pageSize)
        {
            IQueryable<FollowedMovie> query = _followedMovieRepo
                .GetBaseQuery(predicate: fm => fm.UserId == userId && fm.Movie.IsShow == true);


            IEnumerable<FollowedMovie> followedMovies = await query
                .Include(fm => fm.Movie)
                .OrderBy(fm => fm.Movie.Title)
                .Where(fm => fm.Movie.IsShow)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            List<FollowedMovieDTO> followedMovieDTOs = _mapper.Map<List<FollowedMovieDTO>>(followedMovies);

            foreach (var followedMovieDTO in followedMovieDTOs)
            {
                followedMovieDTO.CommentCount = await _commentRepo.CountAsync(c => c.MovieId == followedMovieDTO.Id);
            }

            return Result<List<FollowedMovieDTO>>.Success(followedMovieDTOs, "Followed movies retrieved successfully");
        }

        public async Task<Result<bool>> RateMovie(CreateRateMovieRequestDTO rateMovieRequestDTO)
        {
            Result<bool> result = await _movieService.CreateRate(rateMovieRequestDTO);
            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.Message);
            }
            return Result<bool>.Success(result.Data, result.Message);
        }

        public async Task<Result<List<NotificationDTO>>> GetNotifications(string userId)
        {
            return await _notificationService.GetNotifications(userId);
        }

        public Task<Result<NotificationDTO>> MarkNotificationAsRead(int notificationId)
        {
            return _notificationService.MarkAsRead(notificationId);
        }

        public Task<Result<bool>> DeleteNotification(int notificationId)
        {
            return _notificationService.DeleteNotification(notificationId);
        }

        public async Task<Result<bool>> IsUserBoughtMovie(string userId, int movieId)
        {
            var result = await _orderRepo.FindOneAsync(o => o.UserId == userId && o.MovieId == movieId);
            return Result<bool>.Success(result != null, "Bought status retrieved successfully");
        }

        public async Task<Result<List<MovieDTO>>> PurchasedMovies(string userId, int pageNumber, int pageSize)
        {
            IQueryable<Order> query = _orderRepo.GetBaseQuery(o => o.UserId == userId);

            IEnumerable<Order> orders = await query
                .Include(o => o.Movie)
                .OrderBy(o => o.Movie.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!orders.Any())
            {
                return Result<List<MovieDTO>>.Failure("No movies found for this user.");
            }

            var movies = orders.Select(o => o.Movie).ToList();
            List<MovieDTO> movieDTOs = _mapper.Map<List<MovieDTO>>(movies);

            return Result<List<MovieDTO>>.Success(movieDTOs, "Purchased movies retrieved successfully.");
        }
    }
}
