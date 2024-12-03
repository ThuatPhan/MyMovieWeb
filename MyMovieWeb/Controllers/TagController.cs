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
        private readonly ITagServices _tagServices;
        public TagController(ILogger<TagController> logger, ITagServices tagServices)
        {
            _logger = logger;
            _tagServices = tagServices;
        }

        [HttpPost]
        [Authorize(Policy = "create:data")]
        public async Task<ActionResult<ApiResponse<TagDTO>>> CreateTag([FromBody] TagRequestDTO tagRequestDTO)
        {
            try
            {
                Result<TagDTO> result = await _tagServices.CreateTag(tagRequestDTO);
                return CreatedAtAction(
                    nameof(GetTag),
                    new { id = result.Data.Id },
                    ApiResponse<TagDTO>.SuccessResponse(result.Data, result.Message)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<TagDTO>.FailureResponse("An error occurred while creating tags")
                );
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "update:data")]
        public async Task<ActionResult<ApiResponse<TagDTO>>> UpdateTag([FromRoute] int id, [FromBody] TagRequestDTO tagRequestDTO)
        {
            try
            {
                Result<TagDTO> result = await _tagServices.UpdateTag(id, tagRequestDTO);
                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<TagDTO>.FailureResponse(result.Message));
                }
                return Ok(ApiResponse<TagDTO>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<TagDTO>.FailureResponse("An error occurred while updating tag")
                );
            }
        }
        
        [HttpDelete("{id}")]
        [Authorize(Policy = "delete:data")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteTag([FromRoute] int id)
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
        public async Task<ActionResult<ApiResponse<TagDTO>>> GetTag([FromRoute] int id)
        {
            try
            {
                Result<TagDTO> result = await _tagServices.GetTagById(id);
                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<TagDTO>.FailureResponse($"Tag id {id} not found"));
                }
                return Ok(ApiResponse<TagDTO>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<TagDTO>.FailureResponse("An error occurred while retrieving tag")
                );
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<TagDTO>>>> GetAllTags()
        {
            try
            {
                Result<List<TagDTO>> result = await _tagServices.GetAllTags();
                return Ok(ApiResponse<List<TagDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<List<TagDTO>>.FailureResponse("An error occurred while retrieving tags")
                );
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<ApiResponse<int>>> GetTotalTagCount()
        {
            try
            {
                Result<int> result = await _tagServices.CountTag(_ => true);
                return Ok(ApiResponse<int>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<List<TagDTO>>.FailureResponse("An error occurred while retrieving tags")
                );
            }
        }

        [HttpGet("paged")]
        public async Task<ActionResult<ApiResponse<List<TagDTO>>>> GetAllTags([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                Result<List<TagDTO>> result = await _tagServices.FindAll(
                    predicate: _ => true, 
                    orderBy: t => t.OrderByDescending(t => t.Id), 
                    pageNumber: pageNumber, 
                    pageSize: pageSize
                );
                return Ok(ApiResponse<List<TagDTO>>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<List<TagDTO>>.FailureResponse("An error occurred while retrieving tags")
                );
            }

        }
    }
}

