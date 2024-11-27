using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMovieWeb.Application;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Presentation.Response;

namespace MyMovieWeb.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EpisodeController : ControllerBase
    {
        private readonly ILogger<EpisodeController> _logger;
        private readonly IEpisodeServices _episodeServices;

        public EpisodeController(ILogger<EpisodeController> logger, IEpisodeServices episodeServices)
        {
            _logger = logger;
            _episodeServices = episodeServices;
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        [Authorize (Policy = "create:episode")]
        public async Task<ActionResult<ApiResponse<EpisodeDTO>>> CreateEpisode([FromForm] CreateEpisodeRequestDTO episodeRequestDTO)
        {
            try
            {
                Result<EpisodeDTO> result = await _episodeServices.CreateEpisode(episodeRequestDTO);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<EpisodeDTO>.FailureResponse(result.Message));
                }
                return CreatedAtAction(
                    nameof(GetEpisode),
                    new { id = result.Data.Id },
                    ApiResponse<EpisodeDTO>.SuccessResponse(result.Data, result.Message)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when creating episode");
            }
        }

        [HttpPut("{id}")]
        [DisableRequestSizeLimit]
        [Authorize(Policy = "update:episode")]
        public async Task<ActionResult<ApiResponse<EpisodeDTO>>> UpdateEpisode([FromRoute] int id, [FromForm] UpdateEpisodeRequestDTO episodeRequestDTO)
        {
            try
            {
                Result<EpisodeDTO> result = await _episodeServices.UpdateEpisode(id, episodeRequestDTO);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<EpisodeDTO>.FailureResponse(result.Message));
                }
                return Ok(ApiResponse<EpisodeDTO>.SuccessResponse(result.Data, result.Message));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when updating episode");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "delete:episode")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteEpisode([FromRoute] int id)
        {
            try
            {
                Result<bool> result = await _episodeServices.DeleteEpisode(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.FailureResponse(result.Message));
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when deleting episode");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<EpisodeDTO>>> GetEpisode([FromRoute] int id)
        {
            try
            {
                Result<EpisodeDTO> result = await _episodeServices.GetEpisodeById(id);
                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<EpisodeDTO>.FailureResponse(result.Message));
                }
                return Ok(ApiResponse<EpisodeDTO>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving episode");
            }
        }

        [HttpGet("get-by-movie/{movieId}")]
        public async Task<ActionResult<ApiResponse<List<EpisodeDTO>>>> GetEpisodeOfMovie([FromRoute] int movieId)
        {
            try
            {
                Result<List<EpisodeDTO>> result = await _episodeServices.GetEpisodesOfMovie(movieId);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<List<EpisodeDTO>>.FailureResponse(result.Message));
                }
                return Ok(ApiResponse<List<EpisodeDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving episodes");
            }
        }
    }
}
