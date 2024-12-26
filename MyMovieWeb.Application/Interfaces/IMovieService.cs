using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Utils;
using MyMovieWeb.Domain.Entities;
using System.Linq.Expressions;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IMovieService
    {
        Task<Result<MovieDTO>> CreateMovie(CreateMovieRequestDTO movieRequestDTO);
        Task<Result<MovieDTO>> UpdateMovie(int id, UpdateMovieRequestDTO movieRequestDTO);
        Task<Result<bool>> DeleteMovie(int id);
        Task<Result<int>> CountMovieBy(Expression<Func<Movie, bool>> predicate);
        Task<Result<MovieDTO>> GetMovieById(int id);
        Task<Result<List<MovieDTO>>> FindAllMovies(int pageNumber, int pageSize, Expression<Func<Movie, bool>> predicate, Func<IQueryable<Movie>, IOrderedQueryable<Movie>>? orderBy = null);
        Task<Result<List<MovieDTO>>> GetMoviesSameGenreOfMovie(int movieId, int pageNumber, int pageSize);
        Task<Result<bool>> IncreaseView(int id, int view);
        Task<Result<bool>> CreateRate(CreateRateMovieRequestDTO rateMovieRequestDTO);
        Task<Result<int>> CountTredingInDay();
        Task<Result<List<MovieDTO>>> GetTrendingInDay(int pageNumber, int pageSize);
        Task<Result<List<MovieDTO>>> GetTopView(TimePeriod timePeriod, int topCount);
        Task<Result<List<MovieDTO>>> GetNewComment(int topCount);
        Task<Result<List<MovieDTO>>> SearchMovieByName(string keyword);
        Task<Result<List<MovieDTO>>> GetRecommendedMovies(int movieId, int topMovie);
        Task<Result<List<MovieDTO>>> GetMoviesByOption(bool paidMovie, int pageNumber, int pageSize);
        Task<Result<int>> CountMovieByOption(bool paidMovie);
    }
}
