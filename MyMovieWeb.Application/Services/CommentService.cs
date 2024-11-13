using AutoMapper;
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
        private readonly ICommentRepository _commentRepo;
        private readonly IMovieService _movieServics;
        private readonly IEpisodeServices _episodeServices;
        private readonly IAuth0Services _auth0Services;

        public CommentService(IMapper mapper, IMovieService movieService, IEpisodeServices episodeServices, ICommentRepository commentRepository, IAuth0Services auth0Services)
        {
            _mapper = mapper;
            _movieServics = movieService;
            _episodeServices = episodeServices;
            _commentRepo = commentRepository;
            _auth0Services = auth0Services;
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
            var user = await _auth0Services.GetUser(userId);
            commentDTO.User = user!;

            return Result<CommentDTO>.Success(commentDTO, "Comment created successfully");
        }

        public async Task<Result<CommentDTO>> CreateEpisodeComment(CreateEpisodeCommentRequestDTO commentRequestDTO, string userId)
        {
            Result<MovieDTO> movieResult = await _movieServics.GetMovieById(commentRequestDTO.MovieId);
            Result<EpisodeDTO> episodeResult = await _episodeServices.GetEpisodeById(commentRequestDTO.EpisodeId);
            if(!movieResult.IsSuccess)
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

        public async Task<Result<List<CommentDTO>>> GetCommentsOfMovie(int movieId)
        {
            Result<MovieDTO> result = await _movieServics.GetMovieById(movieId);
            if(!result.IsSuccess)
            {
                return Result<List<CommentDTO>>.Failure(result.Message);
            }

            IEnumerable<Comment> comments = await _commentRepo.GetCommentsByMovieIdAsync(movieId);

            List<CommentDTO> commentDTOs = _mapper.Map<List<CommentDTO>>(comments);
            List<Auth0UserDTO>? users = await _auth0Services.GetAllUsers();

            foreach (var commentDTO in commentDTOs)
            {
                commentDTO.User = users.SingleOrDefault(u => u.UserId == commentDTO.UserId);
            }

            return Result<List<CommentDTO>>.Success(commentDTOs, "Comments retrieved successfully");
        }

        public async Task<Result<List<CommentDTO>>> GetCommentsOfEpisode(int movieId, int episodeId)
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

            IEnumerable<Comment> comments = await _commentRepo.GetCommentsByEpisodeIdAsync(movieId, episodeId);
            List<CommentDTO> commentDTOs = _mapper.Map<List<CommentDTO>>(comments);

            return Result<List<CommentDTO>>.Success(commentDTOs, "Comments retrieved successfully");
        }

    }

}
