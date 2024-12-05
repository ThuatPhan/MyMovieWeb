using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IStatisticServices
    {
        Task<Result<List<ViewChartDTO>>> GetChartMovieViewData(DateTime startDate, DateTime endDate, int numberOfMovie);
    }
}
