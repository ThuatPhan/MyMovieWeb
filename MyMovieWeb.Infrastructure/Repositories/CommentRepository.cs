using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;
using MyMovieWeb.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<Comment>> GetCommentsByEpisodeIdAsync(int movieId, int episodeId)
        {
            return await _dbContext.Comments
                .Where(c => c.MovieId == movieId && c.EpisodeId == episodeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetCommentsByMovieIdAsync(int movieId)
        {
            return await _dbContext.Comments
                .Where(c => c.MovieId == movieId && c.EpisodeId == null)
                .ToListAsync();
        }

    }
}
