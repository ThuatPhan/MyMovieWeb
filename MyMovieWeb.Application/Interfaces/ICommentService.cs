using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieWeb.Application.Interfaces
{
    public interface ICommentService
    {
        Task<Result<CommentDTO>> CreateMovieComment(CreateMovieCommentRequestDTO commentRequestDTO, string userId);
        Task<Result<CommentDTO>> CreateEpisodeComment(CreateEpisodeCommentRequestDTO commentRequestDTO, string userId);
        Task<Result<bool>> DeleteComment(int id);
        Task<Result<List<CommentDTO>>> GetCommentsOfMovie(int movieId);
        Task<Result<List<CommentDTO>>> GetCommentsOfEpisode(int movieId, int episodeId);

    }
}
