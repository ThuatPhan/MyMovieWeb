﻿using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IMovieService
    {
        Task<Result<MovieDTO>> CreateMovie(CreateMovieRequestDTO movieRequestDTO);
        Task<Result<MovieDTO>> UpdateMovie(int id, UpdateMovieRequestDTO movieRequestDTO);
        Task<Result<bool>> DeleteMovie(int id);
        Task<Result<int>> CountMovie();
        Task<Result<int>> CountMovieByGenre(int genreId);
        Task<Result<MovieDTO>> GetMovieById(int id);
        Task<Result<List<MovieDTO>>> GetAllMovies();
        Task<Result<List<MovieDTO>>> GetPagedMovies(int pageNumber, int pageSize, bool? isShow = null);
        Task<Result<List<MovieDTO>>> GetPagedMoviesByGenre(int genreId, int pageNumber, int pageSize);
        Task<Result<List<MovieDTO>>> GetPagedMoviesSameGenre(int movieId, int pageNumber, int pageSize);
        Task<Result<List<MovieDTO>>> GetPagedMoviesRecentAdded(int pageNumber, int pageSize);
        Task<Result<List<MovieDTO>>> GetPagedMovies(int pageNumber, int pageSize);
        Task<Result<List<MovieDTO>>> GetPagedTvShows(int pageNumber, int pageSize);
    }
}
