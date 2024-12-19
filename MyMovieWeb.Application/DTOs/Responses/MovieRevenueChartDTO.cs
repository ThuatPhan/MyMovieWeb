namespace MyMovieWeb.Application.DTOs.Responses
{
    public class MovieRevenueChartDTO
    {
        public MovieDTO Movie { get; set; }
        public long? TotalRevenue { get; set; }
    }
}
