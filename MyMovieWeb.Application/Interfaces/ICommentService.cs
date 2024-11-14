using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;

namespace MyMovieWeb.Application.Interfaces
{
    public interface ICommentService
    {
        Task<Result<CommentDTO>> CreateMovieComment(CreateMovieCommentRequestDTO commentRequestDTO, string userId);
        Task<Result<CommentDTO>> CreateEpisodeComment(CreateEpisodeCommentRequestDTO commentRequestDTO, string userId);
        Task<Result<bool>> DeleteComment(int id);
        Task<Result<int>> CountCommentOfMovie(int movieId);
        Task<Result<int>> CountCommentOfEpisode(int movieId, int episodeId);
        Task<Result<CommentDTO>> GetCommentById(int id);
        Task<Result<List<CommentDTO>>> GetCommentsOfMovie(int movieId, int pageNumber, int pageSize);
        Task<Result<List<CommentDTO>>> GetCommentsOfEpisode(int movieId, int episodeId, int pageNumber, int pageSize);

    }
}
