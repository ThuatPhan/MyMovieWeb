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

        [HttpDelete("{id}")]
        //[Authorize(Policy = "delete:movie")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteMovie([FromRoute] int id)
        {
            try
            {
                Result<bool> result = await _movieServices.DeleteMovie(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<bool>.FailureResponse(result.Message));
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<bool>.FailureResponse("An error occurred when deleting movie")
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

        [HttpGet("count")]
        public async Task<ActionResult<ApiResponse<int>>> GetMovieCount()
        {
            try
            {
                Result<int> result = await _movieServices.CountMovie();
                return ApiResponse<int>.SuccessResponse(result.Data, result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<int>.FailureResponse("An error occurred when counting movies")
                );
            }
        }

        [HttpGet("paged")]
        public async Task<ActionResult<ApiResponse<List<MovieDTO>>>> GetPagedMovies([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                Result<List<MovieDTO>> result = await _movieServices.GetPagedMovies(pageNumber, pageSize, true);
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

        [HttpGet("admin-paged")]
        public async Task<ActionResult<ApiResponse<List<MovieDTO>>>> GetPagedMoviesForAdminPage([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                Result<List<MovieDTO>> result = await _movieServices.GetPagedMovies(pageNumber, pageSize);
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

        [HttpGet("count-by-genre/{genreId}")]
        public async Task<ActionResult<ApiResponse<int>>> GetMovieCountByGenre([FromRoute] int genreId)
        {
            try
            {
                Result<int> result = await _movieServices.CountMovieByGenre(genreId);

                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<int>.FailureResponse(result.Message));
                }

                return Ok(ApiResponse<int>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<int>.FailureResponse("An error occurred when counting movies")
                );
            }
        }

        [HttpGet("get-by-genre/{genreId}")]
        public async Task<ActionResult<ApiResponse<List<MovieDTO>>>> GetMoviesByGenre([FromRoute] int genreId, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                Result<List<MovieDTO>> result = await _movieServices.GetPagedMoviesByGenre(genreId, pageNumber, pageSize);
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

        [HttpGet("get-same-genre")]
        public async Task<ActionResult<ApiResponse<List<MovieDTO>>>> GetMoviesSameGenre([FromQuery] int movieId, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                Result<List<MovieDTO>> result = await _movieServices.GetPagedMoviesSameGenre(movieId, pageNumber, pageSize);
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

        [HttpGet("recent-added")]
        public async Task<ActionResult<ApiResponse<List<MovieDTO>>>> GetRecentAddedMovies([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                Result<List<MovieDTO>> result = await _movieServices.GetPagedMoviesRecentAdded(pageNumber, pageSize);
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
