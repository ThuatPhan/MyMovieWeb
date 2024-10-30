using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMovieWeb.Application;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Presentation.Response;
using System.Security.Claims;

namespace MyMovieWeb.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WatchHistoryController : ControllerBase
    {
        private readonly ILogger<WatchHistoryController> _logger;
        private readonly IWatchHistoryServices _watchHistoryService;

        public WatchHistoryController(ILogger<WatchHistoryController> logger, IWatchHistoryServices watchHistoryService)
        {
            _logger = logger;
            _watchHistoryService = watchHistoryService;
        }

        [HttpPost("movie")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> CreateWatchMovieLog([FromBody] WatchMovieRequestDTO watchMovieRequest)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                Result<bool> result = await _watchHistoryService.CreateWatchMovieLog(watchMovieRequest, userId);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(result.Data, result.Message));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when creating watch movie log");
            }
        }

        [HttpPost("episode")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> CreateWatchEpisodeLog([FromBody] WatchEpisodeRequestDTO watchMovieRequest)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                Result<bool> result = await _watchHistoryService.CreateWatchEpisodeLog(watchMovieRequest, userId);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(result.Data, result.Message));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when creating watch episode log");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<MovieDTO>>>> GetWatchHistoryOfUser()
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                Result<List<MovieDTO>> result = await _watchHistoryService.GetUserWatchHistory(userId);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<EpisodeDTO>>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<List<MovieDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving watch history");
            }
        }
    }
}
