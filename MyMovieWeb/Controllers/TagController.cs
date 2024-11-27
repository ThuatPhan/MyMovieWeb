using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application;
using MyMovieWeb.Presentation.Response;
using MyMovieWeb.Application.Interfaces;

namespace MyMovieWeb.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : Controller
    {
        private readonly ILogger<TagController> _logger;
        private readonly IBlogTagService _tagServices;
        public TagController(ILogger<TagController> logger, IBlogTagService tagServices)
        {
            _logger = logger;
            _tagServices = tagServices;
        }

        [HttpPut("{id}")]
        //[Authorize(Policy = "update:tag")]
        public async Task<ActionResult<ApiResponse<BlogTagDTO>>> UpdateTag([FromRoute] int id, [FromBody] BlogTagRequestDTO tagRequestDTO)
        {
            try
            {
                Result<BlogTagDTO> result = await _tagServices.UpdateTag(id, tagRequestDTO);
                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<BlogTagDTO>.FailureResponse(result.Message));
                }
                return Ok(ApiResponse<BlogTagDTO>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<BlogTagDTO>.FailureResponse("An error occurred while updating tag")
                );
            }
        }
        [HttpPost]
        //[Authorize(Policy = "create:tag")]
        public async Task<ActionResult<ApiResponse<BlogTagDTO>>> CreateTag([FromBody] BlogTagRequestDTO tagRequestDTO)
        {
            try
            {
                Result<BlogTagDTO> result = await _tagServices.CreateTag(tagRequestDTO);
                return CreatedAtAction(
                    nameof(GetGenreById),
                    new { id = result.Data.Id },
                    ApiResponse<BlogTagDTO>.SuccessResponse(result.Data, result.Message)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<BlogTagDTO>.FailureResponse("An error occurred while creating tags")
                );
            }
        }

        [HttpDelete("{id}")]
        //[Authorize(Policy = "delete:tag")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteGenre([FromRoute] int id)
        {
            try
            {
                Result<bool> result = await _tagServices.DeleteTag(id);
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
                    ApiResponse<bool>.FailureResponse("An error occurred while deleting tag")
                );
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<BlogTagDTO>>> GetGenreById([FromRoute] int id)
        {
            try
            {
                Result<BlogTagDTO> result = await _tagServices.GetTagById(id);
                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<BlogTagDTO>.FailureResponse($"Tag id {id} not found"));
                }
                return Ok(ApiResponse<BlogTagDTO>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<BlogTagDTO>.FailureResponse("An error occurred while retrieving tag")
                );
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<BlogTagDTO>>>> GetAllGenres()
        {
            try
            {
                Result<List<BlogTagDTO>> result = await _tagServices.GetAllTag();
                return Ok(ApiResponse<List<BlogTagDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<List<BlogTagDTO>>.FailureResponse("An error occurred while retrieving tags")
                );
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<ApiResponse<int>>> GetTotalTagCount()
        {
            try
            {
                Result<int> result = await _tagServices.CountTag();
                return Ok(ApiResponse<int>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<List<BlogTagDTO>>.FailureResponse("An error occurred while retrieving tags")
                );
            }
        }

        [HttpGet("paged")]
        public async Task<ActionResult<ApiResponse<List<BlogTagDTO>>>> GetGenresPaged([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                Result<List<BlogTagDTO>> result = await _tagServices.GetAllTags(pageNumber, pageSize);
                return Ok(ApiResponse<List<BlogTagDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<List<BlogTagDTO>>.FailureResponse("An error occurred while retrieving tags")
                );
            }

        }
    }
}

