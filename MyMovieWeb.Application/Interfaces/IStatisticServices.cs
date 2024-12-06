using MyMovieWeb.Application.DTOs.Responses;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IStatisticServices
    {
        Task<Result<List<ViewChartDTO>>> GetMovieViewChartData(DateTime startDate, DateTime endDate, int numberOfMovie);
        Task<Result<List<MovieRevenueChartDTO>>> GetMovieRevenueChartData(DateTime dateTime, DateTime endDate, int numberOfMovie);
        Task<Result<List<RevenueChartDTO>>> GetRevenueChartData(DateTime startDate, DateTime endDate);
    }
}
