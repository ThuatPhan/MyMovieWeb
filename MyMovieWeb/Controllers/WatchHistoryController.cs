﻿using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("guest/create-log/movie")]
        public async Task<ActionResult<ApiResponse<WatchHistoryDTO>>> CreateWatchMovieLog(
            [FromBody] WatchMovieRequestDTO watchMovieRequest,
            [FromQuery] string userId)
        {
            try
            {
                Result<WatchHistoryDTO> result = await _watchHistoryService.CreateWatchMovieLog(watchMovieRequest, userId);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<WatchHistoryDTO>.SuccessResponse(result.Data, result.Message));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when creating watch movie log");
            }
        }

        [HttpPost("guest/create-log/episode")]
        public async Task<ActionResult<ApiResponse<WatchHistoryDTO>>> CreateWatchEpisodeLog(
            [FromBody] WatchEpisodeRequestDTO watchMovieRequest,
            [FromQuery] string userId)
        {
            try
            {
                Result<WatchHistoryDTO> result = await _watchHistoryService.CreateWatchEpisodeLog(watchMovieRequest, userId);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<WatchHistoryDTO>.SuccessResponse(result.Data, result.Message));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when creating watch episode log");
            }
        }

        [HttpPost("user/create-log/movie")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<WatchHistoryDTO>>> CreateWatchMovieLog([FromBody] WatchMovieRequestDTO watchMovieRequest)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                Result<WatchHistoryDTO> result = await _watchHistoryService.CreateWatchMovieLog(watchMovieRequest, userId);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<WatchHistoryDTO>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<WatchHistoryDTO>.SuccessResponse(result.Data, result.Message));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when creating watch movie log");
            }
        }

        [HttpPost("user/create-log/episode")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<WatchHistoryDTO>>> CreateWatchEpisodeLog([FromBody] WatchEpisodeRequestDTO watchMovieRequest)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                Result<WatchHistoryDTO> result = await _watchHistoryService.CreateWatchEpisodeLog(watchMovieRequest, userId);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<WatchHistoryDTO>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<WatchHistoryDTO>.SuccessResponse(result.Data, result.Message));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when creating watch episode log");
            }
        }

        [HttpPut("sync-log")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<WatchHistoryDTO>>>> UpdateGuestToUserWatchHistory([FromBody] string guestId)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                Result<List<WatchHistoryDTO>> result = await _watchHistoryService.UpdateGuestToUserWatchHistory(guestId, userId);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<EpisodeDTO>>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<List<WatchHistoryDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when updating watch history");
            }
        }

        [HttpGet("guest/current-watching-time")]
        public async Task<ActionResult<ApiResponse<WatchHistoryDTO>>> GetCurrentWatchingTime(
            [FromQuery] string userId,
            [FromQuery] int movieId,
            [FromQuery] int? episodeId = null)
        {
            try
            {
                Result<WatchHistoryDTO> result = await _watchHistoryService.GetCurrentWatchingTime(userId, movieId, episodeId);
                if (!result.IsSuccess)
                {
                    return Ok(ApiResponse<WatchHistoryDTO>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<WatchHistoryDTO>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving current watching time");
            }
        }

        [HttpGet("guest/log")]
        public async Task<ActionResult<ApiResponse<WatchHistoryDTO>>> GetWatchHistory([FromQuery] int id, [FromQuery] string userId)
        {
            try
            {
                Result<WatchHistoryDTO> result = await _watchHistoryService.GetWatchHistory(id, userId);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<WatchHistoryDTO>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<WatchHistoryDTO>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retriving watch history");
            }
        }

        [HttpGet("guest/logs")]
        public async Task<ActionResult<ApiResponse<List<WatchHistoryDTO>>>> GetWatchHistories(
            [FromQuery] string guestId,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize)
        {
            try
            {
                Result<List<WatchHistoryDTO>> result = await _watchHistoryService.GetWatchHistories(pageNumber, pageSize, guestId);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<EpisodeDTO>>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<List<WatchHistoryDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving watch history");
            }
        }

        [HttpGet("user/current-watching-time")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<WatchHistoryDTO>>> GetCurrentWatchingTime(
            [FromQuery] int movieId,
            [FromQuery] int? episodeId = null)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                Result<WatchHistoryDTO> result = await _watchHistoryService.GetCurrentWatchingTime(userId, movieId, episodeId);
                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<WatchHistoryDTO>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<WatchHistoryDTO>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving current watching time");
            }
        }

        [HttpGet("user/log")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<WatchHistoryDTO>>> GetWatchHistory([FromQuery] int id)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                Result<WatchHistoryDTO> result = await _watchHistoryService.GetWatchHistory(id, userId);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<WatchHistoryDTO>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<WatchHistoryDTO>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retriving watch history");
            }
        }

        [HttpGet("user/logs")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<WatchHistoryDTO>>>> GetWatchHistories(
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                Result<List<WatchHistoryDTO>> result = await _watchHistoryService.GetWatchHistories(pageNumber, pageSize, userId);

                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<EpisodeDTO>>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<List<WatchHistoryDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving watch history");
            }
        }
    }
}
