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
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private readonly IPostServices _postServices;

        public PostController(ILogger<PostController> logger, IPostServices postServices)
        {
            _postServices = postServices;
            _logger = logger;
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        // [Authorize(Policy = "create:post")]
        public async Task<ActionResult<ApiResponse<PostDTO>>> CreatePost([FromForm] CreatePostRequestDTO postRequestDTO)
        {
            try
            {
                Result<PostDTO> result = await _postServices.CreatePost(postRequestDTO);
                return CreatedAtAction(
                    nameof(GetPost),
                    new { id = result.Data.Id },
                    ApiResponse<PostDTO>.SuccessResponse(result.Data, result.Message)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<PostDTO>.FailureResponse("An error occurred when creating Blog post")
                );
            }
        }

        [HttpPut("{id}")]
        [DisableRequestSizeLimit]
        // [Authorize(Policy = "update:post")]
        public async Task<ActionResult<ApiResponse<PostDTO>>> UpdatePost([FromRoute] int id, [FromForm] UpdatePostRequestDTO postRequestDTO)
        {
            try
            {
                Result<PostDTO> result = await _postServices.UpdatePost(id, postRequestDTO);
                if (!result.IsSuccess)
                {
                    return BadRequest(ApiResponse<PostDTO>.FailureResponse(result.Message));
                }
                return Ok(ApiResponse<PostDTO>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<PostDTO>.FailureResponse("An error occurred when Updating Blog post")
                );
            }
        }

        [HttpDelete("{id}")]
        //[Authorize(Policy = "delete:post")]
        public async Task<ActionResult<ApiResponse<bool>>> DeletePost([FromRoute] int id)
        {
            try
            {
                Result<bool> result = await _postServices.DeletePost(id);
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
                    ApiResponse<bool>.FailureResponse("An error occurred when deleting Blog post")
                );
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<PostDTO>>> GetPost([FromRoute] int id)
        {
            try
            {
                Result<PostDTO> result = await _postServices.GetPostById(id);
                if (!result.IsSuccess)
                {
                    return NotFound(ApiResponse<PostDTO>.FailureResponse(result.Message));
                }
                return Ok(ApiResponse<PostDTO>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<PostDTO>.FailureResponse("An error occurred when retrieving Blog")
                );
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<ApiResponse<int>>> GetPostCount([FromQuery] bool includeHiddenPost)
        {
            try
            {
                Result<int> result = includeHiddenPost == true
                    ? await _postServices.CountPost(predicate: _ => true)
                    : await _postServices.CountPost(predicate: p => p.IsShow);
                                                    
                return Ok(ApiResponse<int>.SuccessResponse(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred when retrieving total post count");
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<PostDTO>>>> GetPosts(
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            [FromQuery] bool includeHiddenPost
        )
        {
            try
            {
                Result<List<PostDTO>> result = includeHiddenPost == true
                    ? await _postServices.FindAll(pageNumber, pageSize, predicate: _ => true)
                    : await _postServices.FindAll(pageNumber, pageSize, predicate: p => p.IsShow);

                return Ok(ApiResponse<List<PostDTO>>.SuccessResponse(result.Data, result.Message));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<List<PostDTO>>.FailureResponse("An error occurred when retrieving Posts")
                );
            }
        }
    }
}
