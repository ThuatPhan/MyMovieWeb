using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;

namespace MyMovieWeb.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Comment> _commentRepo;
        private readonly IRepository<FollowedMovie> _followedMovieRepo;
        private readonly IMovieService _movieServics;
        private readonly IEpisodeServices _episodeServices;
        private readonly IAuth0Services _auth0Services;
        private readonly INotificationServices _notificationServices;
        public CommentService(
            IMapper mapper, 
            IRepository<FollowedMovie> followedMovieRepo,
            IMovieService movieService, 
            IEpisodeServices episodeServices, 
            IAuth0Services auth0Services,
            IRepository<Comment> commentRepository,
            INotificationServices notificationServices

        )
        {
            _mapper = mapper;
            _followedMovieRepo = followedMovieRepo;
            _movieServics = movieService;
            _episodeServices = episodeServices;
            _commentRepo = commentRepository;
            _auth0Services = auth0Services;
            _notificationServices = notificationServices;
        }

        public async Task<Result<CommentDTO>> CreateMovieComment(CreateMovieCommentRequestDTO commentRequestDTO, string userId)
        {
            Result<MovieDTO> result = await _movieServics.GetMovieById(commentRequestDTO.MovieId);
            if (!result.IsSuccess)
            {
                return Result<CommentDTO>.Failure(result.Message);
            }

            Comment newComment = _mapper.Map<Comment>(commentRequestDTO);
            newComment.UserId = userId;

            Comment createdComment = await _commentRepo.AddAsync(newComment);
            CommentDTO commentDTO = _mapper.Map<CommentDTO>(createdComment);

            Result<Auth0UserDTO> userResult = await _auth0Services.GetUser(userId);
            if (!userResult.IsSuccess)
            {
                return Result<CommentDTO>.Failure(result.Message);
            }
            commentDTO.User = userResult.Data;

            List<string> userIds = (await _followedMovieRepo.FindAllAsync(fm => fm.MovieId == commentDTO.MovieId))
                .Where(fm => fm.UserId != userId)
                .Select(fm => fm.UserId)
                .ToList();

            await _notificationServices.AddNotifications(userIds, $"Một người dùng đã bình luận phim {result.Data.Title} mà bạn theo dõi", $"/details/{result.Data.Id}");

            return Result<CommentDTO>.Success(commentDTO, "Comment created successfully");
        }

        public async Task<Result<CommentDTO>> CreateEpisodeComment(CreateEpisodeCommentRequestDTO commentRequestDTO, string userId)
        {
            Result<MovieDTO> movieResult = await _movieServics.GetMovieById(commentRequestDTO.MovieId);
            Result<EpisodeDTO> episodeResult = await _episodeServices.GetEpisodeById(commentRequestDTO.EpisodeId);
            if (!movieResult.IsSuccess)
            {
                return Result<CommentDTO>.Failure(movieResult.Message);
            }
            if (!episodeResult.IsSuccess)
            {
                return Result<CommentDTO>.Failure(episodeResult.Message);
            }

            Comment newComment = _mapper.Map<Comment>(commentRequestDTO);
            newComment.UserId = userId;

            Comment createdComment = await _commentRepo.AddAsync(newComment);
            CommentDTO commentDTO = _mapper.Map<CommentDTO>(createdComment);

            Result<Auth0UserDTO> userResult = await _auth0Services.GetUser(userId);
            if (!userResult.IsSuccess)
            {
                return Result<CommentDTO>.Failure(userResult.Message);
            }

            commentDTO.User = userResult.Data;

            return Result<CommentDTO>.Success(commentDTO, "Comment created successfully");
        }

        public async Task<Result<bool>> DeleteComment(int id)
        {
            Comment? commentToDelete = await _commentRepo.GetByIdAsync(id);
            if (commentToDelete is null)
            {
                return Result<bool>.Failure($"Comment id {id} not found");
            }

            await _commentRepo.RemoveAsync(commentToDelete);

            return Result<bool>.Success(true, "Comment deleted successfully");
        }

        public async Task<Result<CommentDTO>> GetCommentById(int id)
        {
            Comment? comment = await _commentRepo.GetByIdAsync(id);
            if (comment is null)
            {
                return Result<CommentDTO>.Failure($"Episode id {id} not found");
            }

            CommentDTO commentDTO = _mapper.Map<CommentDTO>(comment);

            return Result<CommentDTO>.Success(commentDTO, "Comment retrived successfully");

        }

        public async Task<Result<int>> CountCommentOfMovie(int movieId)
        {
            Result<MovieDTO> result = await _movieServics.GetMovieById(movieId);
            if (!result.IsSuccess)
            {
                return Result<int>.Failure(result.Message);
            } 

            int commentCount = await _commentRepo.CountAsync(c => c.MovieId == movieId);
            return Result<int>.Success(commentCount, "Comment count retrieved successfully");
        }

        public async Task<Result<int>> CountCommentOfEpisode(int movieId, int episodeId)
        {
            Result<MovieDTO> movieResult = await _movieServics.GetMovieById(movieId);
            if (!movieResult.IsSuccess)
            {
                return Result<int>.Failure(movieResult.Message);
            }

            Result<EpisodeDTO> episodeResult = await _episodeServices.GetEpisodeById(episodeId);
            if (!episodeResult.IsSuccess)
            {
                return Result<int>.Failure(episodeResult.Message);
            }

            int commentCount = await _commentRepo.CountAsync(c => c.MovieId == movieId && c.EpisodeId == episodeId);
            return Result<int>.Success(commentCount, "Comment count retrieved successfully");
        }

        public async Task<Result<List<CommentDTO>>> GetCommentsOfMovie(int movieId, int pageNumber, int pageSize)
        {
            Result<MovieDTO> result = await _movieServics.GetMovieById(movieId);
            if (!result.IsSuccess)
            {
                return Result<List<CommentDTO>>.Failure(result.Message);
            }

            IQueryable<Comment> query = _commentRepo.GetBaseQuery(predicate: c => c.MovieId == movieId);

            IEnumerable<Comment> comments = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            List<CommentDTO> commentDTOs = _mapper.Map<List<CommentDTO>>(comments);

            Result<List<Auth0UserDTO>> usersResult = await _auth0Services.GetAllUsers();
            if (!usersResult.IsSuccess)
            {
                return Result<List<CommentDTO>>.Failure(usersResult.Message);
            }

            foreach (var commentDTO in commentDTOs)
            {
                commentDTO.User = usersResult.Data.SingleOrDefault(u => u.UserId == commentDTO.UserId);
            }

            return Result<List<CommentDTO>>.Success(commentDTOs, "Comments retrieved successfully");
        }

        public async Task<Result<List<CommentDTO>>> GetCommentsOfEpisode(int movieId, int episodeId, int pageNumber, int pageSize)
        {
            Result<MovieDTO> movieResult = await _movieServics.GetMovieById(movieId);
            Result<EpisodeDTO> episodeResult = await _episodeServices.GetEpisodeById(episodeId);
            if (!movieResult.IsSuccess)
            {
                return Result<List<CommentDTO>>.Failure(movieResult.Message);
            }
            if (!episodeResult.IsSuccess)
            {
                return Result<List<CommentDTO>>.Failure(episodeResult.Message);
            }
            IQueryable<Comment> query = _commentRepo.GetBaseQuery(predicate: c => c.MovieId == movieId && c.EpisodeId == episodeId);

            IEnumerable<Comment> comments = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((pageSize - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            List<CommentDTO> commentDTOs = _mapper.Map<List<CommentDTO>>(comments);

            Result<List<Auth0UserDTO>> usersResult = await _auth0Services.GetAllUsers();
            if (!usersResult.IsSuccess)
            {
                return Result<List<CommentDTO>>.Failure(usersResult.Message);
            }

            foreach (var commentDTO in commentDTOs)
            {
                commentDTO.User = usersResult.Data.SingleOrDefault(u => u.UserId == commentDTO.UserId);
            }

            return Result<List<CommentDTO>>.Success(commentDTOs, "Comments retrieved successfully");
        }

    }

}
