using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMovieWeb.Application;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Application.Utils;
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
        [Authorize(Policy = "create:movie")]
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
        [Authorize(Policy = "update:movie")]
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
        [Authorize(Policy = "delete:movie")]
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
        public async Task<ActionResult<ApiResponse<List<MovieDTO>>>> GetAllMovies(
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            [FromQuery] bool includeHiddenMovie
        )
        {
            try
            {
                Result<List<MovieDTO>> result = includeHiddenMovie == true
                    ? await _movieServices.FindAllMovies(pageNumber, pageSize, _ => true, m => m.OrderBy(m => m.Title))
                    : await _movieServices.FindAllMovies(pageNumber, pageSize, m => m.IsShow, m => m.OrderBy(m => m.Title));

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
        public async Task<ActionResult<ApiResponse<int>>> GetMovieCount([FromQuery] bool includeHiddenMovie)
        {
            try
            {
                Result<int> result = includeHiddenMovie
                    ? await _movieServices.CountMovieBy(_ => true)
                    : await _movieServices.CountMovieBy(m => m.IsShow);

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

        [HttpGet("count-by-genre")]
        public async Task<ActionResult<ApiResponse<int>>> GetMovieCountByGenre(
            [FromQuery] int genreId,
            [FromQuery] bool includeHiddenMovie
        )
        {
            try
            {
                Result<int> result = includeHiddenMovie
                    ? await _movieServices.CountMovieBy(m => m.MovieGenres.Any(mg => mg.GenreId == genreId))
                    : await _movieServices.CountMovieBy(m => m.IsShow && m.MovieGenres.Any(mg => mg.GenreId == genreId));

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

        [HttpGet("get-by-genre")]
        public async Task<ActionResult<ApiResponse<List<MovieDTO>>>> GetMoviesByGenre(
            [FromQuery] int genreId,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize
        )
        {
            try
            {
                Result<List<MovieDTO>> result = await _movieServices
                    .FindAllMovies(
                        pageNumber,
                        pageSize,
                        m => m.IsShow && m.MovieGenres.Any(mg => mg.GenreId == genreId),
                        m => m.OrderBy(m => m.Title)
                    );

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
        public async Task<ActionResult<ApiResponse<List<MovieDTO>>>> GetMoviesSameGenre(
            [FromQuery] int movieId,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize
        )
        {
            try
            {
                Result<List<MovieDTO>> result = await _movieServices.GetMoviesSameGenreOfMovie(movieId, pageNumber, pageSize);
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
        public async Task<ActionResult<ApiResponse<List<MovieDTO>>>> GetRecentAddedMovies(
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize
        )
        {
            try
            {
                Result<List<MovieDTO>> result = await _movieServices
                        .FindAllMovies(
                            pageNumber,
                            pageSize,
                            m => m.IsShow,
                            m => m.OrderBy(m => m.Title).OrderByDescending(m => m.ReleaseDate)
                        );

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

        [HttpGet("tv-shows")]
        public async Task<ActionResult<ApiResponse<List<MovieDTO>>>> GetTvShows(
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize
        )
        {
            try
            {
                Result<List<MovieDTO>> result = await _movieServices
                    .FindAllMovies(
                        pageNumber,
                        pageSize,
                        m => m.IsShow && m.IsSeries, m => m.OrderBy(m => m.Title)
                    );

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

        [HttpGet("movies")]
        public async Task<ActionResult<ApiResponse<List<MovieDTO>>>> GetMovies(
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize
        )
        {
            try
            {
                Result<List<MovieDTO>> result = await _movieServices
                    .FindAllMovies(
                        pageNumber,
                        pageSize,
                        m => m.IsShow && !m.IsSeries, m => m.OrderBy(m => m.Title)
                    );

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

        [HttpGet("top-views")]
        public async Task<ActionResult<ApiResponse<List<MovieDTO>>>> GetTopViews([FromQuery]TimePeriod timePeriod, [FromQuery] int topCount)
        {
            try
            {
                Result<List<MovieDTO>> result = await _movieServices.GetTopView(timePeriod, topCount);
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
        [HttpGet("new-comments")]
        public async Task<ActionResult<ApiResponse<List<MovieDTO>>>> GetMoviesWithNewComments([FromQuery] int topCount)
        {
            try
            {

                Result<List<MovieDTO>> result = await _movieServices.GetNewComment(topCount);
                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<List<MovieDTO>>.FailureResponse(result.Message));
                }
                return Ok(ApiResponse<List<MovieDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<List<MovieDTO>>.FailureResponse("An error occurred when retrieving movies with new comments.")
                );
            }
        }
    }
}
