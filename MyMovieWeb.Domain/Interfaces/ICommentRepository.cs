using MyMovieWeb.Domain.Entities;
using System.Linq.Expressions;

namespace MyMovieWeb.Domain.Interfaces
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<IEnumerable<Comment>> FindAllAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<Comment, bool>> predicate,
            Func<IQueryable<Comment>, IOrderedQueryable<Comment>>? orderBy = null
        );
    }
}
