using System.Linq.Expressions;

namespace MyMovieWeb.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task RemoveAsync(T entity);
        Task<int> CountAsync();
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task RemoveRangeAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> predicate);
    }
}
