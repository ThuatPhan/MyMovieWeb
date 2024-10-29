using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IMovieService
    {
        Task<Result<MovieDTO>> CreateMovie(CreateMovieRequestDTO movieRequestDTO);
        Task<Result<MovieDTO>> UpdateMovie(int id, UpdateMovieRequestDTO movieRequestDTO);
        Task<Result<bool>> DeleteMovie(int id);
        Task<Result<MovieDTO>> GetMovieById(int id);
        Task<Result<List<MovieDTO>>> GetAllMovies();
        Task<Result<int>> GetTotalMovieCount();
        Task<Result<List<MovieDTO>>> GetPagedMovies(int pageNumber, int pageSize);
    }
}
