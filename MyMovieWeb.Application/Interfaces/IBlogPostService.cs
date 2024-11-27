using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IBlogPostService
    {
        Task<Result<BlogPostDTO>> CreateBlogPost(CreateBlogRequestDTO blogRequestDTO);
        Task<Result<BlogPostDTO>> UpdateBlogPost(int id, UpdateBlogRequestDTO blogRequestDTO);
        Task<Result<bool>> DeleteBlogPost(int id);
        Task<Result<BlogPostDTO>> GetBlogPostById(int id);
        Task<Result<List<BlogPostDTO>>> FindAllBlogPost(int pageNumber, int pageSize, Expression<Func<BlogPost, bool>> predicate, Func<IQueryable<BlogPost>, IOrderedQueryable<BlogPost>>? orderBy = null);
    }
}
