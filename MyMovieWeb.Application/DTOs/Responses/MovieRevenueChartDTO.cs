namespace MyMovieWeb.Application.DTOs.Responses
{
    public class MovieRevenueChartDTO
    {
        public MovieDTO Movie { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
