using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IUserServices
    {
        Task<Result<bool>> FollowMovie(int movieId, string userId);
        Task<Result<List<FollowedMovieDTO>>> GetFollowedMovies(string userId);
        Task<Result<bool>> RateMovie(CreateRateMovieRequestDTO rateMovieRequestDTO);
    }
}
