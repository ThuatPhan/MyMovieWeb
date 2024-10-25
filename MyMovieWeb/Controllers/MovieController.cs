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
    public class MovieController : ControllerBase
    {
        private readonly ILogger<MovieController> _logger;
        private readonly IMovieService _movieServices;

        public MovieController(ILogger<MovieController> logger, IMovieService movieService)
        {
            _logger = logger;
            _movieServices = movieService;
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        //[Authorize(Policy = "create:movie")]
        public async Task<ActionResult<ApiResponse<MovieDTO>>> CreateMovie([FromForm] CreateMovieRequestDTO movieRequestDTO)
        {
            try
            {
                Result<MovieDTO> result = await _movieServices.CreateMovie(movieRequestDTO);
                return CreatedAtAction(
                    nameof(GetMovieById),
                    new { id = result.Data.Id },
                    ApiResponse<MovieDTO>.SuccessResponse(result.Data, result.Message)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<MovieDTO>.FailureResponse("An error occurred when creating movies")
                );
            }
        }

        [HttpPut("{id}")]
        [DisableRequestSizeLimit]
        //[Authorize(Policy = "update:movie")]
        public async Task<ActionResult<ApiResponse<MovieDTO>>> UpdateMovie([FromRoute] int id, [FromForm] UpdateMovieRequestDTO movieRequestDTO)
        {
            try
            {
                Result<MovieDTO> result = await _movieServices.UpdateMovie(id, movieRequestDTO);
                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<MovieDTO>.FailureResponse(result.Message));
                }
                return Ok(ApiResponse<MovieDTO>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<MovieDTO>.FailureResponse("An error occurred when Updating movies")
                );
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<MovieDTO>>> GetMovieById([FromRoute] int id)
        {
            try
            {
                Result<MovieDTO> result = await _movieServices.GetMovieById(id);
                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<MovieDTO>.FailureResponse(result.Message));
                }
                return Ok(ApiResponse<MovieDTO>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<MovieDTO>.FailureResponse("An error occurred when retrieving movie")
                );
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<MovieDTO>>>> GetAllMovies()
        {
            try
            {
                Result<List<MovieDTO>> result = await _movieServices.GetAllMovies();
                return Ok(ApiResponse<List<MovieDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<List<MovieDTO>>.FailureResponse("An error occurred when retrieving movies")
                );
            }
        }
    }
}
