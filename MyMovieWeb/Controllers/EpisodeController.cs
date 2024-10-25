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
        public async Task<ActionResult<ApiResponse<EpisodeDTO>>> CreateEpisode([FromForm] CreateEpisodeRequestDTO episodeRequestDTO)
        {
            try
            {
                Result<EpisodeDTO> result = await _episodeServices.CreateEpisode(episodeRequestDTO);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<EpisodeDTO>.FailureResponse(result.Message));
                }

                return CreatedAtAction(nameof(GetEpisode), new { id = result.Data.Id }, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<EpisodeDTO>>> GetEpisode([FromRoute] int id)
        {
            try
            {
                Result<EpisodeDTO> result = await _episodeServices.GetById(id);
                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<EpisodeDTO>.FailureResponse(result.Message));
                }
                return Ok(ApiResponse<EpisodeDTO>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
