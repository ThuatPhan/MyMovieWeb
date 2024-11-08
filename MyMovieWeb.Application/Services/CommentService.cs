using AutoMapper;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Helper;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;
using MyMovieWeb.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieWeb.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly IMapper _mapper;
        private readonly ICommentRepository _commentRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IEpisodeRepository _episodeRepository;
        private readonly FileUploadHelper _uploadHelper;
        public CommentService(IMapper mapper, ICommentRepository commentRepository,IMovieRepository movieRepository, IEpisodeRepository episodeRepository, FileUploadHelper uploadHelper)
        {
            _mapper = mapper;
            _commentRepository = commentRepository;
            _episodeRepository = episodeRepository;
            _uploadHelper = uploadHelper;
            _movieRepository = movieRepository;
        }

        public async Task<Result<CommentDTO>> CreateMovieComment(CreateMovieCommentRequestDTO commentRequestDTO, string userId)
        {
            Comment newComment = _mapper.Map<Comment>(commentRequestDTO);
            Movie? movie = await _movieRepository.GetByIdAsync(commentRequestDTO.MovieId);
            if(movie is null)
            {
                return Result<CommentDTO>.Failure($"Movie Id {commentRequestDTO.MovieId} not found");
            }
            newComment.UserId = userId;
            Comment createdComment = await _commentRepository.AddAsync(newComment);
            CommentDTO commentDTO = _mapper.Map<CommentDTO>(createdComment);

            return Result<CommentDTO>.Success(commentDTO, "Comment created successfully");
        }

        public async Task<Result<CommentDTO>> CreateEpisodeComment(CreateEpisodeCommentRequestDTO commentRequestDTO, string userId)
        {
            Comment newComment = _mapper.Map<Comment>(commentRequestDTO);
            Movie? movie = await _movieRepository.GetByIdAsync(commentRequestDTO.MovieId);
            Episode? episode = await _episodeRepository.GetByIdAsync(commentRequestDTO.EpisodeId);
            if (movie is null)
            {
                return Result<CommentDTO>.Failure($"Movie Id {commentRequestDTO.MovieId} not found");
            }
            if(episode is null)
            {
                return Result<CommentDTO>.Failure($"Episode Id {commentRequestDTO.EpisodeId} not found");
            }
            newComment.UserId = userId;
            Comment createdComment = await _commentRepository.AddAsync(newComment);
            CommentDTO commentDTO = _mapper.Map<CommentDTO>(createdComment);

            return Result<CommentDTO>.Success(commentDTO, "Comment created successfully");
        }
        public async Task<Result<bool>> DeleteComment(int id)
        {
            Comment? commentToDelete = await _commentRepository.GetByIdAsync(id);
            if (commentToDelete is null)
            {
                return Result<bool>.Failure($"Comment id {id} not found");
            }

            await _commentRepository.RemoveAsync(commentToDelete);

            return Result<bool>.Success(true, "Comment deleted successfully");
        }

        public async Task<Result<List<CommentDTO>>> GetCommentsOfEpisode(int movieId, int episodeId)
        {
            Movie? movie = await _movieRepository.GetByIdAsync(movieId);
            Episode? episode = await _episodeRepository.GetByIdAsync(episodeId);
            if (movie is null)
            {
                return Result<List<CommentDTO>>.Failure($"Movie Id {movieId} not found");
            }
            if (episode is null)
            {
                return Result<List<CommentDTO>>.Failure($"Episode Id {episodeId} not found");
            }

            // Lấy danh sách bình luận cho tập phim và bộ phim đó
            IEnumerable<Comment> comments = await _commentRepository.GetCommentsByEpisodeIdAsync(movieId, episodeId);
            List<CommentDTO> commentDTOs = _mapper.Map<List<CommentDTO>>(comments);
            return Result<List<CommentDTO>>.Success(commentDTOs, "Comments retrieved successfully");
        }

        public async Task<Result<List<CommentDTO>>> GetCommentsOfMovie(int movieId)
        {
            var movie = await _movieRepository.GetByIdAsync(movieId);
            if (movie == null)
            {
                return Result<List<CommentDTO>>.Failure($"Movie id {movieId} not found.");
            }
            // Lấy tất cả các bình luận của bộ phim
            IEnumerable<Comment> comments = await _commentRepository.GetCommentsByMovieIdAsync(movieId);
            List<CommentDTO> commentDTOs = _mapper.Map<List<CommentDTO>>(comments);
            return Result<List<CommentDTO>>.Success(commentDTOs, "Comments retrieved successfully.");
        }

    }

}
