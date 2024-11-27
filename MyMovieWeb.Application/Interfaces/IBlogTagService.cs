using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IBlogTagService
    {
        Task<Result<BlogTagDTO>> CreateTag(BlogTagRequestDTO tagRequestDTO);
        Task<Result<BlogTagDTO>> UpdateTag(int id, BlogTagRequestDTO tagRequestDTO);
        Task<Result<bool>> DeleteTag(int id);
        Task<Result<int>> CountTag();
        Task<Result<BlogTagDTO>> GetTagById(int id);
        Task<Result<List<BlogTagDTO>>> GetAllTag();
        Task<Result<List<BlogTagDTO>>> GetAllTags(int pageNumber, int pageSize);
    }
}
