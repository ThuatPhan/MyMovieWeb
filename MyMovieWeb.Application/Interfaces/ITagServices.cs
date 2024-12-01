using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;

namespace MyMovieWeb.Application.Interfaces
{
    public interface ITagServices
    {
        Task<Result<TagDTO>> CreateTag(TagRequestDTO tagRequestDTO);
        Task<Result<TagDTO>> UpdateTag(int id, TagRequestDTO tagRequestDTO);
        Task<Result<bool>> DeleteTag(int id);
        Task<Result<int>> CountTag();
        Task<Result<TagDTO>> GetTagById(int id);
        Task<Result<List<TagDTO>>> GetAllTags();
        Task<Result<List<TagDTO>>> GetAllTags(int pageNumber, int pageSize);
    }
}
