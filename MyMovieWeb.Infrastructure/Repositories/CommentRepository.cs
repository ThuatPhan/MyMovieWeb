using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;
using MyMovieWeb.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieWeb.Infrastructure.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CommentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Comment>> FindAllAsync(
            int pageNumber, 
            int pageSize, 
            Expression<Func<Comment, bool>> predicate, 
            Func<IQueryable<Comment>, IOrderedQueryable<Comment>>? orderBy = null)
        {
            IQueryable<Comment> query = _dbContext.Comments
                .Where(predicate);

            if(orderBy != null)
            {
                query = orderBy(query);
            }

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToListAsync();
        }
    }
}
