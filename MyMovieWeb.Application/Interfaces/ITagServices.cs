using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Domain.Entities;
using System.Linq.Expressions;

namespace MyMovieWeb.Application.Interfaces
{
    public interface ITagServices
    {
        Task<Result<TagDTO>> CreateTag(TagRequestDTO tagRequestDTO);
        Task<Result<TagDTO>> UpdateTag(int id, TagRequestDTO tagRequestDTO);
        Task<Result<bool>> DeleteTag(int id);
        Task<Result<int>> CountTag(Expression<Func<Tag, bool>> predicate);
        Task<Result<TagDTO>> GetTagById(int id);
        Task<Result<List<TagDTO>>> GetAllTags();
        Task<Result<List<TagDTO>>> FindAll(
            int pageNumber, 
            int pageSize, 
            Expression<Func<Tag, bool>> predicate, 
            Func<IQueryable<Tag>, IOrderedQueryable<Tag>>? orderBy = null
        );
    }
}
