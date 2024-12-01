using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Domain.Entities;
using System.Linq.Expressions;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IPostServices
    {
        Task<Result<PostDTO>> CreatePost(CreatePostRequestDTO postRequestDTO);
        Task<Result<PostDTO>> UpdatePost(int id, UpdatePostRequestDTO postRequestDTO);
        Task<Result<bool>> DeletePost(int id);
        Task<Result<PostDTO>> GetPostById(int id);
        Task<Result<List<PostDTO>>> FindAll(
            int pageNumber, 
            int pageSize, 
            Expression<Func<Post, bool>> predicate, 
            Func<IQueryable<Post>, IOrderedQueryable<Post>>? orderBy = null
        );

        Task<Result<int>> CountPost(Expression<Func<Post, bool>> predicate);
    }
}
