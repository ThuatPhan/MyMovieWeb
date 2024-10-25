using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IGenreServices
    {
        Task<Result<GenreDTO>> CreateGenre(GenreRequestDTO genreRequestDTO);
        Task<Result<GenreDTO>> UpdateGenre(int id, GenreRequestDTO genreRequestDTO);
        Task<Result<bool>> DeleteGenre(int id);
        Task<Result<GenreDTO>> GetGenreById(int id);
        Task<Result<List<GenreDTO>>> GetAllGenres();
    }
}
