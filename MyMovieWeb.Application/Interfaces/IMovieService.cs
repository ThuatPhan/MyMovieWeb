using Microsoft.AspNetCore.Http;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IMovieService
    {
        Task<Result<MovieDTO>> CreateMovie(CreateMovieRequestDTO movieRequestDTO);
        Task<Result<MovieDTO>> UpdateMovie(int id, UpdateMovieRequestDTO movieRequestDTO);
        Task<Result<bool>> DeleteMovie(int id);
        Task<Result<MovieDTO>> GetMovieById(int id);
        Task<Result<List<MovieDTO>>> GetAllMovies();
    }
}
