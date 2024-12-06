using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMovieWeb.Application;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Presentation.Response;

namespace MyMovieWeb.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private readonly ILogger<StatisticController> _logger;
        private readonly IStatisticServices _statisticServices;

        public StatisticController(ILogger<StatisticController> logger, IStatisticServices statisticServices)
        {
            _logger = logger;
            _statisticServices = statisticServices;
        }

        [HttpGet("view-chart-data")]
        [Authorize(Policy = "read:data")]
        public async Task<ActionResult<ApiResponse<List<ViewChartDTO>>>> GetMovieViewChartData([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int numberOfMovie)
        {
            try
            {
                Result<List<ViewChartDTO>> result = await _statisticServices.GetMovieViewChartData(startDate, endDate, numberOfMovie);
                return Ok(ApiResponse<List<ViewChartDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving view chart data");
            }
        }

        [HttpGet("movie-revenue-chart-data")]
        [Authorize(Policy = "read:data")]
        public async Task<ActionResult<ApiResponse<List<MovieRevenueChartDTO>>>> GetMovieRevenueChartData([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int numberOfMovie)
        {
            try
            {
               Result<List<MovieRevenueChartDTO>> result = await _statisticServices.GetMovieRevenueChartData(startDate, endDate, numberOfMovie);
                return Ok(ApiResponse<List<MovieRevenueChartDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving movie revenue chart data");
            }
        }

        [HttpGet("revenue-chart-data")]
        [Authorize(Policy = "read:data")]
        public async Task<ActionResult<ApiResponse<List<RevenueChartDTO>>>> GetRevenueChartData([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                Result<List<RevenueChartDTO>> result = await _statisticServices.GetRevenueChartData(startDate, endDate);
                return Ok(ApiResponse<List<RevenueChartDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving revenue chart data");
            }
        }
    }
}
