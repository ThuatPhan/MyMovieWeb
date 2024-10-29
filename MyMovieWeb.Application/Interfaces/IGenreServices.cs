using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IGenreServices
    {
        Task<Result<GenreDTO>> CreateGenre(GenreRequestDTO genreRequestDTO);
        Task<Result<GenreDTO>> UpdateGenre(int id, GenreRequestDTO genreRequestDTO);
        Task<Result<bool>> DeleteGenre(int id);
        Task<Result<GenreDTO>> GetGenreById(int id);
        Task<Result<List<GenreDTO>>> GetAllGenres();
        Task<Result<int>> GetTotalCountGenres();
        Task<Result<List<GenreDTO>>> GetPagedGenres(int pageNumber, int pageSize);

    }
}
