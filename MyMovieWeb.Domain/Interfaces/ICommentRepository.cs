using MyMovieWeb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieWeb.Domain.Interfaces
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetCommentsByMovieIdAsync(int movieId);
        Task<IEnumerable<Comment>> GetCommentsByEpisodeIdAsync(int movieId, int episodeId);

    }
}
