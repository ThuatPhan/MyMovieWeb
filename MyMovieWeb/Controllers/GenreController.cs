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
    public class GenreController : ControllerBase
    {
        private readonly ILogger<GenreController> _logger;
        private readonly IGenreServices _genreServices;

        public GenreController(ILogger<GenreController> logger, IGenreServices genreServices)
        {
            _logger = logger;
            _genreServices = genreServices;
        }

        [HttpPost]
        //[Authorize(Policy = "create:genre")]
        public async Task<ActionResult<ApiResponse<GenreDTO>>> CreateGenre([FromBody] GenreRequestDTO genreRequestDTO)
        {
            try
            {
                Result<GenreDTO> result = await _genreServices.CreateGenre(genreRequestDTO);
                return CreatedAtAction(
                    nameof(GetGenreById),
                    new { id = result.Data.Id },
                    ApiResponse<GenreDTO>.SuccessResponse(result.Data, result.Message)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<GenreDTO>.FailureResponse("An error occurred while creating genres")
                );
            }
        }

        [HttpPut("{id}")]
        //[Authorize(Policy = "update:genre")]
        public async Task<ActionResult<ApiResponse<GenreDTO>>> UpdateGenre([FromRoute] int id, [FromBody] GenreRequestDTO genreRequestDTO)
        {
            try
            {
                Result<GenreDTO> result = await _genreServices.UpdateGenre(id, genreRequestDTO);
                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<GenreDTO>.FailureResponse(result.Message));
                }
                return Ok(ApiResponse<GenreDTO>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<GenreDTO>.FailureResponse("An error occurred while updating genre")
                );
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<GenreDTO>>> GetGenreById([FromRoute] int id)
        {
            try
            {
                Result<GenreDTO> result = await _genreServices.GetGenreById(id);
                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<GenreDTO>.FailureResponse($"Genre id {id} not found"));
                }
                return Ok(ApiResponse<GenreDTO>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<GenreDTO>.FailureResponse("An error occurred while retrieving genre")
                );
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<GenreDTO>>>> GetAllGenres()
        {
            try
            {
                Result<List<GenreDTO>> result = await _genreServices.GetAllGenres();
                return Ok(ApiResponse<List<GenreDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<List<GenreDTO>>.FailureResponse("An error occurred while retrieving genres")
                );
            }
        }
    }
}
